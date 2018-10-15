namespace Sales.Common.Models
{
    using Newtonsoft.Json;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        [StringLength(50)]
        public string Description { get; set; }

        [DataType(DataType.MultilineText)]
        public string Remarks { get; set; }

        [Display(Name = "Image")]
        public string ImagePath { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal Price { get; set; }

        [Display(Name = "Is Available")]
        public bool IsAvailable { get; set; }

        [Display(Name = "Publish On")]
        [DataType(DataType.Date)]
        public DateTime PublishOn { get; set; }

        [Required]
        [StringLength(128)]
        public string UserId { get; set; }


        [NotMapped]
        public byte[] ImageArray { get; set; }

        //clave foranea tcategoryId:
        public int CategoryId { get; set; }

        //relacion con la clase category:
        //lado varios
        [JsonIgnore]//es para que este campo no sea tenido encuenta en la desarializacion
        public virtual Category Category { get; set; }

        public string ImageFullPath
        {
            get
            {
                if (string.IsNullOrEmpty(ImagePath))
                {
                    return "noproduct";
                }

                return $"http://192.168.0.11:54268/{ImagePath.Substring(1)}";
                //return $"https://salesapiservices.azurewebsites.net/{ImagePath.Substring(1)}";
               // return $"https://salesbackend.azurewebsites.net/{ImagePath.Substring(1)}";
            }

        }
    }
}
