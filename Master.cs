namespace XmlImport
{
    public class Master
    {
        public int CIK { get; set; }
        public string CompanyName { get; set; }
        public string FormType { get; set; }
        public string DateField { get; set; }
        public string FileName { get; set; }
        public Master(int cik, string companyName, string formType, string dateField, string fileName)
        {
            CIK = cik;
            CompanyName = companyName;
            FormType = formType;
            DateField = dateField;
            FileName = fileName;
        }       

    }
}
