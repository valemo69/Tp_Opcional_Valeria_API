using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Dominio;


namespace Negocio
{
    public class ArticuloNegocio
    {
        public List<Articulos> listar()
        {
            List<Articulos> lista = new List<Articulos>();
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("SELECT A.ID, A.Codigo, A.Nombre, A.Descripcion AS DescripcionArticulo," +
                    "A.Precio, A.IdMarca, A.IdCategoria, " +
                    "C.Descripcion AS Tipo, " +
                    "M.Descripcion AS Marca " +
                    "FROM ARTICULOS A JOIN CATEGORIAS C ON A.IdCategoria = C.Id JOIN MARCAS M ON A.IdMarca = M.Id");
                datos.ejecutarLectura();

                while (datos.Lector.Read())
                {
                    System.Diagnostics.Debug.WriteLine(datos.Lector[0].ToString());
                    Articulos aux = new Articulos();

                    aux.ID = (int)datos.Lector["ID"];
                    aux.Codigo = (string)datos.leerColumna("Codigo");
                    aux.Nombre = (string)datos.leerColumna("Nombre");
                    aux.Descripcion = (string)datos.leerColumna("DescripcionArticulo");

                    if (!(datos.Lector["Precio"] is DBNull))
                        aux.Precio = (decimal)datos.Lector["Precio"];
                    else
                        aux.Precio = 0;

                    aux.Marca = new Marca();
                    aux.Marca.ID = (int)datos.Lector["IdMarca"];
                    aux.Marca.Descripcion = (string)datos.leerColumna("Marca");


                    aux.Categoria = new Categoria();
                    aux.Categoria.ID = (int)datos.Lector["IdCategoria"];
                    aux.Categoria.Descripcion = (string)datos.leerColumna("Tipo");

                    ImagenNegocio imgNegocio = new ImagenNegocio();
                    aux.Imagenes = imgNegocio.listarImagenes(aux.ID);

                    System.Diagnostics.Debug.WriteLine(
                        "ID: " + aux.ID +
                        " Nombre: " + aux.Nombre);

                    lista.Add(aux);

                }

                return lista;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                datos.cerraConexion();
            }
        }

        public void agregar(Articulos nuevo)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("Insert into ARTICULOS(Codigo, Nombre, Descripcion, Precio,IdMarca,IdCategoria) values('" + nuevo.Codigo + "','" + nuevo.Nombre + "','" + nuevo.Descripcion + "'," + nuevo.Precio + ",@idMarca,@idCategoria)");
                datos.setearParametro("@idMarca", nuevo.Marca.ID);
                datos.setearParametro("@idCategoria", nuevo.Categoria.ID);
                datos.ejecutarAccion();
                datos.cerraConexion();

                datos.setearConsulta("SELECT MAX(Id) AS Id FROM ARTICULOS");
                datos.ejecutarLectura();

                int idGenerado = 0;

                if (datos.Lector.Read())
                {
                    idGenerado = (int)datos.Lector["Id"];
                }
                datos.cerraConexion();

                if (nuevo.Imagenes != null && nuevo.Imagenes.Count > 0)
                {
                    ImagenNegocio imgNegocio = new ImagenNegocio();

                    foreach (var img in nuevo.Imagenes)
                    {
                        img.IdArticulo = idGenerado;
                        imgNegocio.AgregarImagen(img);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                datos.cerraConexion();
            }
        }

        public void modificar(Articulos articulo)
        {
            AccesoDatos datos = new AccesoDatos();
            ImagenNegocio imgNegocio = new ImagenNegocio();
            try
            {
                datos.setearConsulta("UPDATE ARTICULOS SET Codigo = @Codigo, Nombre = @Nombre, Descripcion = @Descripcion, Precio = @Precio, IdMarca = @IdMarca, IdCategoria = @IdCategoria WHERE Id = @Id");
                datos.setearParametro("@Codigo", articulo.Codigo);
                datos.setearParametro("@Nombre", articulo.Nombre);
                datos.setearParametro("@Descripcion", articulo.Descripcion);
                datos.setearParametro("@Precio", articulo.Precio);
                datos.setearParametro("@IdMarca", articulo.Marca.ID);
                datos.setearParametro("@IdCategoria", articulo.Categoria.ID);
                datos.setearParametro("@Id", articulo.ID);
                datos.ejecutarAccion();
                datos.cerraConexion();

                imgNegocio.eliminarImagen(articulo.ID);

                foreach (var img in articulo.Imagenes)
                {
                    imgNegocio.AgregarImagen(img);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                datos.cerraConexion();
            }
        }
        public void eliminar(int ID)
        {
            ImagenNegocio imgNegocio = new ImagenNegocio();
            AccesoDatos datosArticulos = new AccesoDatos();
            try
            {
                imgNegocio.eliminarImagen(ID);

                datosArticulos.setearConsulta("DELETE FROM ARTICULOS WHERE Id = @Id");
                datosArticulos.setearParametro("@Id", ID);
                datosArticulos.ejecutarAccion();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                datosArticulos.cerraConexion();
            }
        }
        public List<Articulos> filtrar(string campo, string criterio, string filtro)
        {
            List<Articulos> lista = new List<Articulos>();
            AccesoDatos datos = new AccesoDatos();

            try
            {
                string consulta = "SELECT A.Id, A.Codigo, A.Nombre, A.Descripcion, A.Precio, A.IdMarca, A.IdCategoria, C.Descripcion AS Tipo, M.Descripcion AS Marca FROM ARTICULOS A JOIN CATEGORIAS C ON A.IdCategoria = C.Id JOIN MARCAS M ON A.IdMarca = M.Id WHERE ";

                if (campo == "Precio")
                {
                    switch (criterio)
                    {
                        case "Mayor a":
                            consulta += "A.Precio > " + filtro;
                            break;
                        case "Menor a":
                            consulta += "A.Precio < " + filtro;
                            break;
                        default:
                            consulta += "A.Precio = " + filtro;
                            break;
                    }
                }
                else if (campo == "Marcas")
                {
                    switch (criterio)
                    {
                        case "Comienza con":
                            consulta += "M.Descripcion LIKE '" + filtro + "%'";
                            break;
                        case "Termina con":
                            consulta += "M.Descripcion LIKE '%" + filtro + "'";
                            break;
                        default:
                            consulta += "M.Descripcion LIKE '%" + filtro + "%'";
                            break;
                    }
                }
                else
                {
                    switch (criterio)
                    {
                        case "Comienza con":
                            consulta += "C.Descripcion LIKE '" + filtro + "%'";
                            break;
                        case "Termina con":
                            consulta += "C.Descripcion LIKE '%" + filtro + "'";
                            break;
                        default:
                            consulta += "C.Descripcion LIKE '%" + filtro + "%'";
                            break;
                    }
                }

                datos.setearConsulta(consulta);
                datos.ejecutarLectura();

                while (datos.Lector.Read())
                {
                    Articulos aux = new Articulos();

                    aux.ID = (int)datos.Lector["Id"];
                    aux.Codigo = (string)datos.Lector["Codigo"];
                    aux.Nombre = (string)datos.Lector["Nombre"];
                    aux.Descripcion = (string)datos.Lector["Descripcion"];

                    if (!(datos.Lector["Precio"] is DBNull))
                        aux.Precio = (decimal)datos.Lector["Precio"];
                    else
                        aux.Precio = 0;
                    
                    aux.Marca = new Marca();
                    aux.Marca.ID = (int)datos.Lector["IdMarca"];
                    aux.Marca.Descripcion = (string)datos.leerColumna("Marca");

                    aux.Categoria = new Categoria();
                    aux.Categoria.ID = (int)datos.Lector["IdCategoria"];
                    aux.Categoria.Descripcion = (string)datos.leerColumna("Tipo");

                    ImagenNegocio imgNegocio = new ImagenNegocio();
                    aux.Imagenes = imgNegocio.listarImagenes(aux.ID);

                    lista.Add(aux);
                }

                return lista;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                datos.cerraConexion();
            }
        }
        //metodo que valida la exixtencia de un articulo por su codigo, para evitar duplicados//correccion del tp anterior
        public bool existeCodigo(string codigo)
        {
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta("SELECT Codigo FROM ARTICULOS WHERE Codigo = @codigo");

                datos.setearParametro("@codigo", codigo);

                datos.ejecutarLectura();

                return datos.Lector.Read();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                datos.cerraConexion();
            }
        }
    }
}
