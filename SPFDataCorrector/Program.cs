using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SPFDataCorrector
{
    class Program
    {
        public const byte SSP_STX = 0x7F;
        public const byte SENSOR_DATA_LENGTH = 50;

        static void Main(string[] args)
        {
            string WorkingFolder = args[0];
            string[] FileList = Directory.GetFiles(WorkingFolder, "*.spd");

            foreach (string FileName in FileList)
            {
                //List<List<byte>> SensorDataList = new List<List<byte>>();
                byte[] FileContents = File.ReadAllBytes(FileName);
                byte[] NewFileContents = new byte[FileContents.Length]; 
                int FileIndex = 0;
                while (FileIndex < FileContents.Length)
                {
                    List<byte> SensorData = new List<byte>();
                    byte[] SensorDataBytes = new byte[SENSOR_DATA_LENGTH];
                    Array.Copy(FileContents, FileIndex, SensorDataBytes, 0, SENSOR_DATA_LENGTH);
                    SensorData.AddRange(SensorDataBytes);
                    RemoveByteStuffing(SensorData);
                    SensorDataBytes = SensorData.ToArray();
                    Array.Copy(SensorDataBytes, 0, NewFileContents, FileIndex, SENSOR_DATA_LENGTH);
                    FileIndex += SENSOR_DATA_LENGTH;
                }


                File.WriteAllBytes( FileName + ".new" , NewFileContents);
                string CsvFileContents = ByteArrayToCsv(NewFileContents);
                File.AppendAllText("SensorData.csv", CsvFileContents + Environment.NewLine);
            

            }

        }

        
        static void RemoveByteStuffing(List<byte> sensorData)
        {
            for (int i = 1; i < sensorData.Count; i++)    // Loop through the list.
            {
                if (sensorData[i] == SSP_STX && sensorData[i - 1] == SSP_STX)    // If consecutive bytes match the stuffed byte.
                {
                    sensorData.RemoveAt(i);    // Remove one of them.
                    sensorData.Add(0x00);    // Add zero at end to keep list length constant.
                }
            }
        }

        public static string ByteArrayToCsvPadded(byte[] byteArray)
        {
            StringBuilder sbCsv = new StringBuilder(byteArray.Length * 5);
            sbCsv.Append(byteArray[0].ToString("000"));   // First element not preceded by ', ' 
            for (int i = 1; i < byteArray.Length; i++)
            {
                sbCsv.Append(", ");
                sbCsv.Append(byteArray[i].ToString("000"));

            }
            return sbCsv.ToString();

        }

        public static string ByteArrayToCsv(byte[] byteArray)
        {
            StringBuilder sbCsv = new StringBuilder(byteArray.Length * 5);
            sbCsv.Append(byteArray[0].ToString());   // First element not preceded by ', ' 
            for (int i = 1; i < byteArray.Length; i++)
            {
                sbCsv.Append(", ");
                sbCsv.Append(byteArray[i].ToString());

            }
            return sbCsv.ToString();

        }
    }
}
