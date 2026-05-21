using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CatalogoAPI.Models
{
    public class ImagenDTO
    {
        // ID del producto al que pertenecen las imágenes
        public int IdArticulo { get; set; }

        // Lista de URLs de imágenes
        public List<string> Imagenes { get; set; }
    }
}