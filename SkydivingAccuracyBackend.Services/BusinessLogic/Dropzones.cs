using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using SkydivingAccuracyBackend.Data.Model;
using SkydivingAccuracyBackend.Services.Data;

namespace SkydivingAccuracyBackend.Services.BusinessLogic
{
    public static class Dropzones
    {
        private static List<Dropzone> _dropzoneList;

        public static void Load()
        {
            var assembly = Assembly.GetEntryAssembly();

            using (var reader = new StreamReader(assembly.GetManifestResourceStream("SkydivingAccuracyBackend.Services.Dropzones.csv")))
            {
                var csvReader = new CsvReader(reader, new CsvConfiguration() { Delimiter = "|" });

                csvReader.Configuration.RegisterClassMap<DropzoneMap>();
                _dropzoneList = csvReader.GetRecords<Dropzone>().ToList();
            }
        }

        public static List<Dropzone> GetAll()
        {
            return _dropzoneList;
        }
    }
}
