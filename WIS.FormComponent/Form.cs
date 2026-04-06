using System;
using System.Collections.Generic;
using System.Linq;

namespace WIS.FormComponent
{
    public class Form
    {
        public string Id { get; set; }
        public List<FormField> Fields { get; set; }
        public List<FormButton> Buttons { get; set; }

        public Form()
        {
            this.Fields = new List<FormField>();
            this.Buttons = new List<FormButton>();
        }
        public Form(string id)
        {
            this.Id = id;
            this.Fields = new List<FormField>();
            this.Buttons = new List<FormButton>();
        }

        public FormField GetField(string id)
        {
            return this.Fields.Where(d => d.Id == id).FirstOrDefault();
        }

        public FormButton GetButton(string id)
        {
            return this.Buttons.Where(d => d.Id == id).FirstOrDefault();
        }

        public bool IsValid()
        {
            return !this.Fields.Any(f => !f.IsValid());
        }
    }
}
