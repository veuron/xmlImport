using System.ComponentModel.DataAnnotations;

namespace XmlImport
{
    public class Master
    {
        public int Id{ get; set; }
        public string CIK { get; set; }
        public string CompanyName { get; set; }
        public string FormType { get; set; }
        public string DateField { get; set; }
        public string FileName { get; set; }

        public Master()
        {

        }
        public Master(string cik, string companyName, string formType, string dateField, string fileName)
        {
            CIK = cik;
            CompanyName = companyName;
            FormType = formType;
            DateField = dateField;
            FileName = fileName;
        }

        public override bool Equals(object? obj)
        {
            if (obj is Master master) return CIK == master.CIK && CompanyName == master.CompanyName && FormType == master.FormType;
            return false;
        }
    }
}
