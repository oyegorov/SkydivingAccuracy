using CsvHelper.Configuration;
using SkydivingAccuracyBackend.Data.Model;

namespace SkydivingAccuracyBackend.Services.Data
{
    public sealed class DropzoneMap : CsvClassMap<Dropzone>
    {
        public DropzoneMap()
        {
            Map(m => m.Name).Name("dz_name");
            Map(m => m.Location).Name("dz_location");
            Map(m => m.Longitude).Name("lon");
            Map(m => m.Latitude).Name("lat");
            Map(m => m.Address).Name("dz_address");
            Map(m => m.Phone).Name("dz_phone");
            Map(m => m.Email).Name("dz_email");
            Map(m => m.Url).Name("dz_url");
        }
    }
}
