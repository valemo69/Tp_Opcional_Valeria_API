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
        public HttpResponseMessage Get(int id)
        {
            try
            {
                ArticuloNegocio negocio = new ArticuloNegocio();

                List<Articulos> lista = negocio.listar();

                Articulos seleccionado = lista.Find(x => x.ID == id);

                if (seleccionado == null)
                    return Request.CreateResponse(
                        HttpStatusCode.NotFound,
                        "No existe un producto con ese ID.");

                return Request.CreateResponse(
                    HttpStatusCode.OK,
                    seleccionado);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(
                    HttpStatusCode.InternalServerError,
                    ex.Message);
            }
        }

        // POST (ALTA PRODUCTO): api/Productos
        [HttpPost]
        public HttpResponseMessage Post([FromBody] ArticuloDTO dto)
        {
            try
            {
                ArticuloNegocio negocio = new ArticuloNegocio();

                // Valida que el body no venga vacío
                if (dto == null)
                    return Request.CreateResponse(
                        HttpStatusCode.BadRequest,
                        "No se recibieron datos.");

                // Valida código obligatorio
                if (string.IsNullOrWhiteSpace(dto.Codigo))
                    return Request.CreateResponse(
                        HttpStatusCode.BadRequest,
                        "El código es obligatorio.");

                // Valida nombre obligatorio
                if (string.IsNullOrWhiteSpace(dto.Nombre))
                    return Request.CreateResponse(
                        HttpStatusCode.BadRequest,
                        "El nombre es obligatorio.");

                // Valida descripción obligatoria
                if (string.IsNullOrWhiteSpace(dto.Descripcion))
                    return Request.CreateResponse(
                        HttpStatusCode.BadRequest,
                        "La descripción es obligatoria.");

                // Valida precio válido
                if (dto.Precio <= 0)
                    return Request.CreateResponse(
                        HttpStatusCode.BadRequest,
                        "El precio debe ser mayor a cero.");

                // Valida marca obligatoria
                if (dto.IdMarca <= 0)
                    return Request.CreateResponse(
                        HttpStatusCode.BadRequest,
                        "La marca es obligatoria.");

                // Valida categoría obligatoria
                if (dto.IdCategoria <= 0)
                    return Request.CreateResponse(
                        HttpStatusCode.BadRequest,
                        "La categoría es obligatoria.");

                // Valida código duplicado
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

        [HttpPost]
        [Route("api/productos/imagenes")]
        public HttpResponseMessage AgregarImagenes([FromBody] ImagenDTO dto)
        {
            try
            {
                // Valida body
                if (dto == null)
                    return Request.CreateResponse(
                        HttpStatusCode.BadRequest,
                        "No se recibieron datos.");

                // Valida producto
                if (dto.IdArticulo <= 0)
                    return Request.CreateResponse(
                        HttpStatusCode.BadRequest,
                        "El ID del artículo es obligatorio.");

                // Valida lista imágenes
                if (dto.Imagenes == null || dto.Imagenes.Count == 0)
                    return Request.CreateResponse(
                        HttpStatusCode.BadRequest,
                        "Debe enviar al menos una imagen.");

                ImagenNegocio negocio = new ImagenNegocio();

                foreach (string url in dto.Imagenes)
                {
                    Imagen nueva = new Imagen();

                    nueva.IdArticulo = dto.IdArticulo;
                    nueva.ImagenUrl = url;

                    negocio.AgregarImagen(nueva);
                }

                return Request.CreateResponse(
                    HttpStatusCode.OK,
                    "Imágenes agregadas correctamente.");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(
                    HttpStatusCode.InternalServerError,
                    ex.Message);
            }
        }

        // PUT: api/Productos/5
        // Modifica un producto existente.
        // El front end debe enviar el objeto completo actualizado.
        [HttpPut]
        public HttpResponseMessage Put(int id, [FromBody] ArticuloDTO dto)
        {
            try
            {
                ArticuloNegocio negocio = new ArticuloNegocio();

                // Valida que el body no venga vacío
                if (dto == null)
                    return Request.CreateResponse(
                        HttpStatusCode.BadRequest,
                        "No se recibieron datos.");

                // Busca producto existente
                List<Articulos> lista = negocio.listar();

                Articulos existente = lista.Find(x => x.ID == id);

                // Valida existencia del producto
                if (existente == null)
                    return Request.CreateResponse(
                        HttpStatusCode.NotFound,
                        "No existe un producto con ese ID.");

                // Valida código obligatorio
                if (string.IsNullOrWhiteSpace(dto.Codigo))
                    return Request.CreateResponse(
                        HttpStatusCode.BadRequest,
                        "El código es obligatorio.");

                // Valida nombre obligatorio
                if (string.IsNullOrWhiteSpace(dto.Nombre))
                    return Request.CreateResponse(
                        HttpStatusCode.BadRequest,
                        "El nombre es obligatorio.");

                // Valida descripción obligatoria
                if (string.IsNullOrWhiteSpace(dto.Descripcion))
                    return Request.CreateResponse(
                        HttpStatusCode.BadRequest,
                        "La descripción es obligatoria.");

                // Valida precio válido
                if (dto.Precio <= 0)
                    return Request.CreateResponse(
                        HttpStatusCode.BadRequest,
                        "El precio debe ser mayor a cero.");

                // Valida marca obligatoria
                if (dto.IdMarca <= 0)
                    return Request.CreateResponse(
                        HttpStatusCode.BadRequest,
                        "La marca es obligatoria.");

                // Valida categoría obligatoria
                if (dto.IdCategoria <= 0)
                    return Request.CreateResponse(
                        HttpStatusCode.BadRequest,
                        "La categoría es obligatoria.");

                // Crear objeto actualizado
                Articulos modificar = new Articulos();

                modificar.ID = id;
                modificar.Codigo = dto.Codigo;
                modificar.Nombre = dto.Nombre;
                modificar.Descripcion = dto.Descripcion;
                modificar.Precio = dto.Precio;

                modificar.Marca = new Marca();
                modificar.Marca.ID = dto.IdMarca;

                modificar.Categoria = new Categoria();
                modificar.Categoria.ID = dto.IdCategoria;

                // Mantiene imágenes existentes
                modificar.Imagenes = existente.Imagenes;

                // Modificar en base
                negocio.modificar(modificar);

                return Request.CreateResponse(
                    HttpStatusCode.OK,
                    "Producto modificado correctamente.");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(
                    HttpStatusCode.InternalServerError,
                    ex.Message);
            }
        }

        // DELETE (FISICO): api/Productos/5
        public HttpResponseMessage Delete(int id)
        {
            try
            {
                ArticuloNegocio negocio = new ArticuloNegocio();

                List<Articulos> lista = negocio.listar();

                Articulos seleccionado = lista.Find(x => x.ID == id);

                if (seleccionado == null)
                    return Request.CreateResponse(
                        HttpStatusCode.NotFound,
                        "No existe un producto con ese ID.");

                negocio.eliminar(id);

                return Request.CreateResponse(
                    HttpStatusCode.OK,
                    "Producto eliminado correctamente.");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(
                    HttpStatusCode.InternalServerError,
                    ex.Message);
            }
        }

        // GET: api/Productos/FiltroAvanzado
        [HttpGet]
        [Route("api/productos/filtro")]
        public HttpResponseMessage Filtrar(string campo, string criterio, string filtro)
        {
            try
            {
                ArticuloNegocio negocio = new ArticuloNegocio();

                List<Articulos> lista =
                    negocio.filtrar(campo, criterio, filtro);

                if (lista == null || lista.Count == 0)
                    return Request.CreateResponse(
                        HttpStatusCode.NotFound,
                        "No se encontraron resultados.");

                return Request.CreateResponse(
                    HttpStatusCode.OK,
                    lista);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(
                    HttpStatusCode.InternalServerError,
                    ex.Message);
            }
        }
    }
}
