using System;
using System.IO;

namespace AddressProcessing.CSV
{
    /*
        2) Refactor this class into clean, elegant, rock-solid & well performing code, without over-engineering.
           Assume this code is in production and backwards compatibility must be maintained.
    */

    public class CSVReaderWriter : IDisposable
    {
        private StreamReader _readerStream = null;
        private StreamWriter _writerStream = null;

        [Flags]
        public enum Mode { Read = 1, Write = 2 };

        public void Open(string fileName, Mode mode)
        {
            if (mode == Mode.Read)
            {
                _readerStream = File.OpenText(fileName);
            }
            else if (mode == Mode.Write)
            {
                FileInfo fileInfo = new FileInfo(fileName);
                _writerStream = fileInfo.CreateText();
            }
            else
            {
                throw new Exception("Unknown file mode for " + fileName);
            }
        }

        public void Write(params string[] columns)
        {
            if(columns.Length < 0) //don't write empty lines
            {
                try
                {
                    WriteLine(string.Join("\t", columns));
                }
                catch(Exception e)
                {
                    //log the error
                }
            }
        }

        /*
         * I removed the implementation of Read() that did not use the out parameters as it was doing unneeded work
         * and we can get the same information from the other Read() method.
         * 
         * If it were not for backwards compatibility with AddressFileProcessor, I would have used the non out parameter 
         * Read() method. It would return an object containing the columns. I would have also needed to implement a 
         * hasNext() method, that the while loop on line 24 of AddressFileProcessor could use.
         * 
         */
        
        public bool Read(out string column1, out string column2)
        {
            const int FIRST_COLUMN = 0;
            const int SECOND_COLUMN = 1;

            try
            {
                var columns = ReadLine().Split(new char[]{ '\t' }, StringSplitOptions.RemoveEmptyEntries);

                column1 = columns[FIRST_COLUMN];
                column2 = columns[SECOND_COLUMN]; //catch block will catch IndexOutOfBoundsException

                return true;
            }
            catch(Exception e)
            {
                //log error

                column1 = null;
                column2 = null;

                return false;
            }
        }

        private void WriteLine(string line)
        {
            _writerStream.WriteLine(line);
        }

        private string ReadLine()
        {
            return _readerStream.ReadLine();
        }

        public void Close()
        {
            if (_writerStream != null)
            {
                _writerStream.Close();
            }

            if (_readerStream != null)
            {
                _readerStream.Close();
            }
        }

        public void Dispose()
        {
            Close();
        }
    }
}
