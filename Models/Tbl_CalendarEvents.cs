using System.ComponentModel.DataAnnotations;

namespace ChurchAPI.Models
{

    public class CalendarEvents
    {
            [Key]
            public int ID { get; set; }   // Primary key

            [Required]
            public string EventName { get; set; } = string.Empty;

            [Required]
            [DataType(DataType.Date)]
            public DateTime EventDate { get; set; }

            
            public string? EventType { get; set; }
            public int CreatedBy { get; set; }
            public DateTime CreatedDate { get; set; }
            public int ChurchId { get; set; }



    }
    public class DailyReadings
    {
        [Key]
        public int ID { get; set; }  

        [Required]
        public string Reading { get; set; } = string.Empty;
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int ChurchId { get; set; }
    }
    public class NoticeMaster
    {
        public long NoticeId { get; set; }
        public string? NoticeName { get; set; }
        public int? DioceseId { get; set; }
        public int? ChurchId { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public bool IsActive { get; set; } = true;
    }

}
