using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace eva.core.framework.framework
{
    /// <summary>
    /// A Generic text file reader
    /// </summary>
    /// <typeparam name="T">the object to which the rows read are to be converted</typeparam>
    public class TextFileReader<T>
    {
        /// <summary>
        /// the file path
        /// </summary>
        private string path;
        /// <summary>
        /// the headers in the file
        /// </summary>
        private string[] header;

        /// <summary>
        /// constructor taking the file path
        /// </summary>
        /// <param name="filepath">the path of the file</param>
        public TextFileReader(string filepath)
        {
            path = filepath;
        }

        /// <summary>
        /// A method which reads the file
        /// </summary>
        /// <returns>an Ienumerble of the rows read ie returns each row at a time</returns>
        public IEnumerable<String[]> readFile()
        {
            // open stream pointing to file
            using (var reader=new StreamReader(path))
            {
                string line = null;
                bool isheader = true;
                // start reading the file
                while ((line=reader.ReadLine()) !=null)
                {
                    //if its a header save it and continue
                    if (isheader)
                    {
                        header = line.Split(',');
                        isheader = false;
                        continue;
                    }
                    //return every row read line by line
                    yield return line.Split(',');
                }
            }
        }

        /// <summary>
        /// This method reads each row and converts it into the specified object
        /// </summary>
        /// <param name="converter">an anonymous function which takes the file headers as an input and the
        /// row read as an inpt and converts them into objects of the specified type</param>
        /// <returns>a list of objects which are read from the file</returns>
        public IEnumerable<T> ReadFileAndConvert(Func<String[], String[], T> converter)
        {
            var fileContents = readFile();
            foreach (var row  in fileContents)
            {
                // convert every row into an object and return it one by one
                yield return converter(row, header);
            }
        }
    }
}
