using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Dominio;
using Negocio;
using CatalogoAPI.Models;

namespace CatalogoAPI.Controllers
{
    public class ProductosController : ApiController
    {
        // GET: api/Productos
        public IEnumerable<Articulos> Get()// algo iterable
        {
            ArticuloNegocio negocio = new ArticuloNegocio();

            return negocio.listar();
        }

        // GET: api/Productos/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Productos
        [HttpPost]
        public HttpResponseMessage Post([FromBody] ArticuloDTO dto)
        {
            try
            {
                ArticuloNegocio negocio = new ArticuloNegocio();

                // Validación simple
                if (dto == null)
                    return Request.CreateResponse(
                        HttpStatusCode.BadRequest,
                        "No se recibieron datos.");

                if (dto.Nombre == "")
                    return Request.CreateResponse(
                        HttpStatusCode.BadRequest,
                        "El nombre es obligatorio.");
                //llamo al metodo existecodigo
                if (negocio.existeCodigo(dto.Codigo))
                    return Request.CreateResponse(
                        HttpStatusCode.Conflict,
                        "El código del producto ya existe.");

                // Crear objeto dominio
                Articulos nuevo = new Articulos();

                nuevo.Codigo = dto.Codigo;

                nuevo.Nombre = dto.Nombre;

                nuevo.Descripcion = dto.Descripcion;

                nuevo.Precio = dto.Precio;

                // Se cargan solo IDs
                nuevo.Marca = new Marca();
                nuevo.Marca.ID = dto.IdMarca;

                nuevo.Categoria = new Categoria();
                nuevo.Categoria.ID = dto.IdCategoria;

                // Guardar en base
                negocio.agregar(nuevo);

                // Respuesta OK
                return Request.CreateResponse(
                    HttpStatusCode.OK,
                    "Producto agregado correctamente.");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(
                    HttpStatusCode.InternalServerError,
                    ex.Message);
            }
        }

        // PUT: api/Productos/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Productos/5
        public void Delete(int id)
        {
        }
    }
}
