namespace WIS.Automation.Galys
{
    public class NotificacionAjusteStockGalysRequest
    {
        public int secuencia { get; set; }  
        
        public string codAlmacen { get; set; }

        public string codArticulo { get; set; }

        public decimal cantidad { get; set; }

        public string lote { get; set; }

        public string caducidad { get; set; }

        public string codigoCausa { get; set; }

        public string usuario { get; set; }

        public string puesto { get; set; }
    }
}
