using System;

namespace CollaborateSoftware.MyLittleHelpers.Backend.Data
{
    public class ToDoExportSettings
    {
        public bool Today { get; set; }
        public bool Open { get; set; }
        public string CategoryId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}
