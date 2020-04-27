using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace OBM.Models.Contact
{
    public class Contact
    {
        [Required(ErrorMessage ="Please Enter Your Name.")]
        [Display(Name = "Your Name: ")]
        public string SenderName { get; set; }

        [Required(ErrorMessage ="Please Enter Valid Email.")]
        [EmailAddress]
        [Display(Name ="Your Email: ")]
        public string SenderEmail { get; set; }

        [Required(ErrorMessage ="Subject is Required.")]
        [Display(Name ="Subject: ")]
        public string EmailSubject { get; set; }

        [Required(ErrorMessage ="Body is Required.")]
        [Display(Name = "Reason: ")]
        public string EmailBody { get; set; }
    }
}