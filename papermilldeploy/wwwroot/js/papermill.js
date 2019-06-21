function initialize() {
        // Variable to store your files
        var files;    
        var ruleCount = 0;
        var batchId = 0;
    
        // back button changes
        $("#back-button").click(function() {
            $("#controls").show();
            document.getElementById("headline_finder").style.display = "block";
            document.getElementById("headline_solutions").style.display = "none";
            $("#jsGrid").jsGrid("option", "data", []);
            $("#jsGrid").jsGrid("option", "fields", []);
            $("#jsGrid").hide();
            $("#re-run").hide();
            files = null;
            $("#input_file").val("");
            ruleCount = 0;
            $("#add-rule").hide();
            $("#back-button").hide();
        });
        
        $( "#run_button" ).click(function() {
            uploadFiles();
        });
        
        // Add events
        $('#input_file').on('change', prepareUpload);

        // Grab the files and set them to our variable
        function prepareUpload(event)
        {
            files = event.target.files;
        }

        // function that handles file uploads
        function uploadFiles() {
            $("#controls").hide();
            $("#spinner").show();
            // Create a formdata object and add the files
            var data = new FormData();
            $.each(files, function(key, value)
            {
                data.append(key, value);
            });

            $.ajax({
                url: $("#run_url").val(),
                type: 'POST',
                data: data,
                cache: false,
                dataType: 'json',
                processData: false, // Don't process the files
                contentType: false, // Set content type to false as jQuery will tell the server its a query string request
                success: function(data, textStatus, jqXHR)
                {
                    createTable(data);
                    document.getElementById("headline_finder").style.display = "none";
                    document.getElementById("headline_solutions").style.display = "block";
                    $("#back-button").show();
                    $("#jsGrid").jsGrid("sort", 0);
                    $("#spinner").hide();
                    $("#add-rule").show();
                    document.getElementById("jsGrid").style.display = "block";
                },
                error: function(jqXHR, textStatus, errorThrown)
                {
                    // Handle errors here
                    console.log('ERRORS: ' + textStatus);
                    // STOP LOADING SPINNER
                }
            });
        }

        //overlay on
        function overlayon() {
            document.getElementById("overlay").style.display = "block";
        }

        //overlay off
        function overlayoff() {
            document.getElementById("overlay").style.display = "none";
            $("#jsGrid-solutions").jsGrid("option", "data", []);
            $("#jsGrid-solutions").jsGrid("option", "fields", []);
            $("#jsGrid-solutions").hide();
        }

        // function responsible to handle overlay changes
        $("#overlay").click(function(event) {
            var parents = $(event.target).parents("div");
            var flag = false;
            $.each(parents, function(index) {
                if (parents[index].className.includes("jsgrid-pager") 
                    || parents[index].className.includes("jsgrid-grid-body") 
                    || parents[index].className.includes("jsgrid-grid-header")) {
                    flag = true;
                    return;
                }
            });
            if (flag)
                return;
            overlayoff();
        });

        $("#add-rule").click(function(event) {
            $("#re-run").show();
            $("#rule-equality").show();
            getObjectives();
        });

    // ajax call responsible to fetch objectives
    function getObjectives() {
        $.ajax({
            url: $("#fetch_objs").val(),
            type: 'GET',
            cache: false,
            success: function (data) {
                addRuleInputDropDown(JSON.parse(data));
            },
            error: function(msg) {
            }
        });
    }

    // function responsible to add rules drop down
    function addRuleInputDropDown(data) {
                var ruleDiv = document.createElement("div");
                ruleDiv.id = "rule-div-" + ruleCount;
                ruleDiv.className = "ruleDivStyle";
                var objDropDown = document.createElement("select");
                objDropDown.id = "rule-obj-"+ruleCount;
                $.each(data.objs, function(index) {
                    objDropDown.options[objDropDown.options.length] = new Option(data.objs[index], data.objs[index]);
                });

                
                var equalityDropDown = document.createElement("select");
                equalityDropDown.id = "equality-" + ruleCount;
                equalityDropDown.options[equalityDropDown.options.length] = new Option("<=", "1");
                equalityDropDown.options[equalityDropDown.options.length] = new Option(">=","2" );

                var inputBox = document.createElement("input");
                inputBox.type = "text";
                inputBox.id = "input-rule-value-" + ruleCount;

                objDropDown.className="ruleDiveleStyle general_fonts eva_buttons";
                equalityDropDown.className="ruleDiveleStyle general_fonts eva_buttons";
                inputBox.className="ruleDiveleStyle general_fonts eva_buttons";
                $("#input-rules").append(ruleDiv);
                $("#rule-div-" + ruleCount).append(objDropDown);
                $("#rule-div-" + ruleCount).append(equalityDropDown);
                $("#rule-div-" + ruleCount).append(inputBox);
                
                ruleCount++;

                for (var i = 0; i < ruleCount; i++) {
                    $("#rule-div-" + i).show();
                    $("#rule-obj-" + i).show();
                    $("#equality-" + i).show();
                    $("#input-rule-value-" + i).show();
                }
            }

            // re-run button click handler
            $("#re-run").click(function() {
                var rules = [];
                for (var i = 0; i < ruleCount; i++) {
                    var createdRule = {
                        lhs:$("#rule-obj-" + i + " option:selected").text(),
                        rhs:$("#input-rule-value-"+i).val(),
                        equality:$("#equality-" + i + " option:selected").text()};
                   
                    rules.push(createdRule);
                    $("#rule-div-" + i).hide();
                }
                
                $("#jsGrid").jsGrid("option", "data", []);
                $("#jsGrid").jsGrid("option", "fields", []);
                $("#jsGrid").hide();
                $("#re-run").hide();
                files = null;
                $("#add-rule").hide();
                $("#spinner").show();
                ruleCount = 0;
                var fdata = new FormData();
                fdata.append("batchId",batchId);
                fdata.append("ruleDef", JSON.stringify(rules));
                $.ajax({
                    url: $("#run_rules").val(),
                    type: 'POST',
                        data: fdata,
                    cache: false,
                    dataType: 'json',
                    processData: false, 
                    contentType: false, // Set content type to false as jQuery will tell the server its a query string request
                    success: function(data)
                    {
                        createTable(data);
                        $("#re-run").show();
                        $("#spinner").hide();
                        $("#add-rule").show();
                        $("#jsGrid").show();
                    },
                    error: function(msg)
                    {
                    }
                });
            });

    //grid required to display details of one particular solution
    function createSolutionGrid(response) {
        var objs = [];
        //var jmsg = JSON.parse(response);
        var jmsg = response;

        $.each(jmsg[0].details, function(index3) {
            var ele = {name: jmsg[0].details[index3], type:"number",width:25}
            objs.push(ele);
        });


        var modifiedSolutions = [];
        $.each(jmsg, function(index) {
            $.each(jmsg[index].values, function(index2) {
                modifiedSolutions.push(jmsg[index].values[index2]);
            });
                       
        });

        $("#jsGrid-solutions").jsGrid({
            paging: true,
            pageSize: 7,
            pageButtonCount: 3,
            autoload: true,
            data: modifiedSolutions,
            fields: objs
                         
        });
    }

    //function responsible to create the js grid based on which solutions to be displayed
    function createTable(msg) {
        var objs = [];
        //var jmsg = JSON.parse(msg);
        var jmsg = msg;
        $.each(jmsg, function(index) {
            $.each(jmsg.slns[0].objectiveEvaluation, function(index3) {
                var ele = {name: jmsg.slns[0].objectiveEvaluation[index3].objectiveName, type:"number",width:25}
                objs.push(ele);
            });
            return false;
        });

        var iniflds = [
            { name: "solutionid", type: "number", width: 25, validate: "required" },
            { name: "batchid", type: "number", width: 25, validate: "required" }
        ];
        var flds = iniflds.concat(objs);

        var modifiedSolutions = [];

        $.each(jmsg.slns, function(index2) {
            var currentSln = jmsg.slns[index2];
            var ele = {solutionid:currentSln.solutionid,batchid : currentSln.batchid};
            $.each(currentSln.objectiveEvaluation, function(index3) {
                var currentObj = currentSln.objectiveEvaluation[index3];
                ele[currentObj.objectiveName] = currentObj.evaluationResult;
            });
            modifiedSolutions.push(ele);
        });

        batchId = modifiedSolutions[0].batchid;

        $("#jsGrid").jsGrid({
            sorting: true,
            paging: true,
            pageSize: 7,
            pageButtonCount: 3,
            autoload: true,
            data: modifiedSolutions,
 
            fields: flds,
            rowClick: function(e) {
                getSlnDetails(e.item.solutionid, e.item.batchid);
            } 
        });
    }

    // ajax call to fetch details of a particular solution
    function getSlnDetails(slnId,batchId) {
        $("#spinner").show();
        $.ajax({
            type: "GET",
            url: $("#fetch_url").val(),
            data: { 
                solutionId: slnId, 
                batchId: batchId
            },
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                createSolutionGrid(response);
                $("#spinner").hide();
                $("#jsGrid-solutions").show();
                overlayon();
            },
            error: function (e) {
                alert("fail");
            }
        });
    };
}
// initialize the document
$(document).ready(function () {
    initialize();
});