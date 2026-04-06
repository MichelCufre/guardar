using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Persistence.Database
{

    public enum EInputType { Unknown, Field, FieldSelect, FieldSelectAsync, FieldCheckbox, FieldDate }

    public class GeneratorBase : Attribute
    {
        public EInputType typeInput { get; set; }
        public bool required { get; set; }
        public string entitySearch { get; set; }
        public string nameEnum { get; set; }
        public string nameProp { get; set; }

    }

    public class Generator : GeneratorBase
    {

        public Generator(string nameProp,bool required = false)
        {
            this.nameProp = nameProp;
            this.required = required;
        }

    }

    public class BGenerator : GeneratorBase
    {

        public BGenerator(string nameProp, bool required = false)
        {
            this.nameProp = nameProp;
            this.required = required;
            this.typeInput = EInputType.FieldCheckbox;
        }

    }
    public class EGenerator : GeneratorBase
    {

        public EGenerator(string nameProp, bool required = false)
        {
            this.nameProp = nameProp;
            this.nameEnum = "E" + nameProp;
            this.required = required;
        }

    }

    public class LGenerator : GeneratorBase
    {

        public LGenerator(string nameProp,string entity, bool isAsync = true, bool required = false)
        {
            this.nameProp = nameProp;
            this.typeInput = isAsync ? EInputType.FieldSelectAsync : EInputType.FieldSelect;
            this.required = required;
            this.entitySearch = entity;
        }
    }

  
    public class PropertyBase : Attribute
    {
        public EInputType typeInput { get; set; }
        public bool required { get; set; }
        public string nameEnum { get; set; }
        public string nameProp { get; set; }

    }

    public class Property : PropertyBase //Este es para especificar propiedades, no genera codigo
    {
        public Property(string nameProp, bool required = false)
        {
            this.nameProp = nameProp;
            this.required = required;
        }

    }

    public class BProperty : PropertyBase
    {

        public BProperty(string nameProp, bool required = false)
        {
            this.nameProp = nameProp;
            this.required = required;
            this.typeInput = EInputType.FieldCheckbox;
        }

    }
    public class EProperty : PropertyBase
    {

        public EProperty(string nameProp, bool required = false)
        {
            this.nameProp = nameProp;
            this.nameEnum = "E" + nameProp;
            this.required = required;
        }

    }

}

