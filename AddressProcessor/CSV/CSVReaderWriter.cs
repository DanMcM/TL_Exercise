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

        //new implementations do not need to use the Mode enum
        public void OpenReader(string fileName)
        {
            try
            {
                _readerStream = File.OpenText(fileName);
            }
            catch(Exception e)
            {
                //log the exception
                //throw appropriate exception
            }
        }

        //new implementations do not need to use the Mode enum
        public void OpenWriter(string fileName)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(fileName);
                _writerStream = fileInfo.CreateText();
            }
            catch (Exception e)
            {
                //log the exception
                //throw appropriate exception
            }
        }

        //kept for backwards compatibility
        public void Open(string fileName, Mode mode)
        {
            try
            {
                if (mode == Mode.Read)
                {
                    OpenReader(fileName);
                }
                else if (mode == Mode.Write)
                {
                    OpenWriter(fileName);
                }
                else
                {
                    throw new Exception("Unknown file mode for " + fileName);
                }
            }
            catch(Exception e)
            {
                //log the exception
                
                //logic for specific exceptions (e.g. FileNotFound)
            }
        }

        public void Write(params string[] columns)
        {
            if(columns.Length <= 0) //don't write empty lines
            {
                try
                {
                    WriteLine(string.Join("\t", columns));
                }
                catch(Exception e)
                {
                    //log the exception
                }
            }
        }

        /*
         * Wasn't sure what exactly this was supposed to do. Original did not return any data.
         * 
         * It now checks that there was actually data read
         */
        public bool Read(string column1, string column2)
        {
            try
            {
                return !string.IsNullOrEmpty(ReadLine().Trim());
            }
            catch (Exception e)
            {
                //log the exception
                return false;
            }
        }

        public bool Read(out string column1, out string column2)
        {
            const int FIRST_COLUMN = 0;
            const int SECOND_COLUMN = 1;

            try
            {
                var columns = ReadLine().Split(new char[]{ '\t' }, StringSplitOptions.RemoveEmptyEntries); //catch block will catch all exceptions

                column1 = columns[FIRST_COLUMN];
                column2 = columns[SECOND_COLUMN]; //catch block will catch IndexOutOfBoundsException

                return true;
            }
            catch(Exception e)
            {
                //log the exception

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

        //user of the class can now close the writer without closing the reader
        public void CloseWriter()
        {
            if (_writerStream != null)
            {
                _writerStream.Close();
            }
        }

        //user of the class can now close the reader without closing the writer
        public void CloseReader()
        {
            if (_readerStream != null)
            {
                _readerStream.Close();
            }
        }

        //kept for backwards compatibility
        public void Close()
        {
            CloseWriter();
            CloseReader();
        }

        //ensures the streams are closed
        public void Dispose()
        {
            Close();
        }
    }
}