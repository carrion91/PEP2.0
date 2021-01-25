﻿using Entidades;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccesoDatos
{
    /// <summary>
    /// Kevin Picado
    /// 07/febrero/2020
    /// Clase para administrar las consultas sql del Ejecucion
    /// </summary>
    public class EjecucionDatos
    {
        private ConexionDatos conexion = new ConexionDatos();
        /// <summary>
        /// Inserta una EjecucionUnidad
        /// </summary>
        /// <param name="unidad">Unidad</param>
        ///  <param name="numeroReferencia"></param>
        ///  <param name="respuesta"></param>
        public void insertarEjecucionUnidad(Unidad unidad, string numeroReferencia,int respuesta)
        {


            SqlConnection sqlConnection = conexion.conexionPEP();

            String consulta = @"insert Unidad_ejecucion (numero_referencia,id_unidad,nombre_unidad,id_ejecucion)
                                            values(@numero_referencia,@id_unidad,@nombre_unidad,@id_ejecucion)";

            SqlCommand command = new SqlCommand(consulta, sqlConnection);

            command.Parameters.AddWithValue("@numero_referencia", Convert.ToString(numeroReferencia));
            command.Parameters.AddWithValue("@id_unidad", unidad.idUnidad);
            command.Parameters.AddWithValue("@nombre_unidad", unidad.nombreUnidad);
            command.Parameters.AddWithValue("@id_ejecucion", respuesta);

            sqlConnection.Open();
            command.ExecuteReader();
            sqlConnection.Close();
        }


        /// <summary>
        /// Inserta una PartidaEjecucion
        /// </summary>
        /// <param name="partida">partida</param>
        /// <param name="numeroReferencia"></param>
        /// <param name="respuesta"></param>
        public void insertarEjecucionPartidas(Partida partida,string numeroReferencia, int respuesta)
        {
            SqlConnection sqlConnection = conexion.conexionPEP();

            String consulta = @"insert Partida_ejecucion (numero_referencia,id_partida,numero_partida,descripcion_partida,id_ejecucion) 
                                            values(@numero_referencia,@id_partida,@numero_partida,@descripcion_partida,@id_ejecucion)";

            SqlCommand command = new SqlCommand(consulta, sqlConnection);

            command.Parameters.AddWithValue("@numero_referencia", Convert.ToString(numeroReferencia));
            command.Parameters.AddWithValue("@id_partida", partida.idPartida);
            command.Parameters.AddWithValue("@numero_partida", partida.numeroPartida);
            command.Parameters.AddWithValue("@descripcion_partida", partida.descripcionPartida);
            command.Parameters.AddWithValue("@id_ejecucion", respuesta);



            sqlConnection.Open();
            command.ExecuteReader();
            sqlConnection.Close();
        }


        /// <summary>
        /// Inserta EjecucionMontoPartidaElegida
        /// </summary>
        /// <param name="partidaUnidad">PartidaUnidad</param>
        /// <param name="numeroReferencia"></param>
        /// <param name="respuesta"></param>
        public void insertarEjecucionMontoPartidaElegida(PartidaUnidad partidaUnidad, string numeroReferencia, int respuesta)
        {
            SqlConnection sqlConnection = conexion.conexionPEP();

            String consulta = @"insert EjecucionMontoPartidasElegidas (numero_referencia,id_partida,id_unidad,monto,monto_disponible,numero_partida,id_ejecucion) 
                                            values(@numero_referencia,@id_partida,@id_unidad,@monto,@monto_disponible,@numero_partida,@id_ejecucion)";

            SqlCommand command = new SqlCommand(consulta, sqlConnection);

            command.Parameters.AddWithValue("@numero_referencia", Convert.ToString(numeroReferencia));
            command.Parameters.AddWithValue("@id_partida", partidaUnidad.IdPartida);
            command.Parameters.AddWithValue("@id_unidad", partidaUnidad.IdUnidad);
            command.Parameters.AddWithValue("@monto", partidaUnidad.Monto);
            command.Parameters.AddWithValue("@monto_disponible", partidaUnidad.MontoDisponible);
            command.Parameters.AddWithValue("@numero_partida", partidaUnidad.NumeroPartida);
            command.Parameters.AddWithValue("@id_ejecucion", respuesta);


            sqlConnection.Open();
            command.ExecuteReader();
            sqlConnection.Close();
        }
        /// <summary>
        /// Inserta Ejecucion
        /// </summary>
        /// <param name="ejecucion">ejecucion</param>
        public int insertarEjecucion(Ejecucion ejecucion)
        {
            SqlConnection sqlConnection = conexion.conexionPEP();
            int respuesta = 0;
            sqlConnection.Open();
            String consulta = @"insert Ejecucion(id_estado,ano_periodo,id_proyecto,monto,id_tipo_tramite,numero_referencia,descripcion_tramite_otro,realizado_por,fecha) output INSERTED.id_ejecucion
                                            values(@id_estado,@ano_periodo,@id_proyecto,@monto,@id_tipo_tramite,@numero_referencia,@descripcion_tramite_otro,@realizadoPor,@fecha)";

            SqlCommand command = new SqlCommand(consulta, sqlConnection);

            command.Parameters.AddWithValue("@id_estado", ejecucion.idestado.idEstado);
            command.Parameters.AddWithValue("@ano_periodo", ejecucion.anoPeriodo);
            command.Parameters.AddWithValue("@id_proyecto", ejecucion.idProyecto);
            command.Parameters.AddWithValue("@monto", ejecucion.monto);
            command.Parameters.AddWithValue("@id_tipo_tramite", ejecucion.idTipoTramite.idTramite);
            command.Parameters.AddWithValue("@numero_referencia", ejecucion.numeroReferencia);
            command.Parameters.AddWithValue("@descripcion_tramite_otro", ejecucion.descripcionEjecucionOtro);
            command.Parameters.AddWithValue("@realizadoPor", ejecucion.realizadoPor);
            command.Parameters.AddWithValue("@fecha", DateTime.Now);

            respuesta = (int)command.ExecuteScalar();

            
            sqlConnection.Close();
            return respuesta;
        }


        /// <summary>
        /// Eliminar una EjecucionUnidad
        /// </summary>
        /// <param name="respuesta">respuesta</param>
        public void EliminarEjecucionUnidad(int respuesta)
        {


            SqlConnection sqlConnection = conexion.conexionPEP();

            String consulta = @"delete  Unidad_ejecucion where id_ejecucion=@id_ejecucion";

            SqlCommand command = new SqlCommand(consulta, sqlConnection);

            command.Parameters.AddWithValue("@id_ejecucion", respuesta);


            sqlConnection.Open();
            command.ExecuteReader();
            sqlConnection.Close();
        }
        /// <summary>
        /// Eliminar una PartidaEjecucion
        /// </summary>
        /// <param name="respuesta"></param>
        public void eliminarEjecucionPartidas( int respuesta)
        {
            SqlConnection sqlConnection = conexion.conexionPEP();

            String consulta = @"delete Partida_ejecucion where id_ejecucion=@id_ejecucion" ;
                                           

            SqlCommand command = new SqlCommand(consulta, sqlConnection);

            
           
            command.Parameters.AddWithValue("@id_ejecucion", respuesta);



            sqlConnection.Open();
            command.ExecuteReader();
            sqlConnection.Close();
        }
        /// <summary>
        /// Eliminar EjecucionMontoPartidaElegida
        /// </summary>
        /// <param name="respuesta"></param>
        public void eliminarEjecucionMontoPartidaElegida(int respuesta)
        {
            SqlConnection sqlConnection = conexion.conexionPEP();

            String consulta = @"delete EjecucionMontoPartidasElegidas where id_ejecucion=@id_ejecucion"; 
                                        

            SqlCommand command = new SqlCommand(consulta, sqlConnection);

           
           
            command.Parameters.AddWithValue("@id_ejecucion", respuesta);


            sqlConnection.Open();
            command.ExecuteReader();
            sqlConnection.Close();
        }
        /// <summary>
        /// Actulizar Ejecucion
        /// </summary>
        /// <param name="ejecucion">ejecucion</param>
        /// <param name="respuesta"></param>
        public void actualizarEjecucion(Ejecucion ejecucion)
        {
            SqlConnection sqlConnection = conexion.conexionPEP();

            String consulta = @"update Ejecucion set id_estado=@id_estado,ano_periodo=@ano_periodo ,id_proyecto=@id_proyecto,monto=@monto,id_tipo_tramite=@id_tipo_tramite,numero_referencia=@numero_referencia,descripcion_tramite_otro=@descripcion_tramite_otro
                                 where id_ejecucion=@id_ejecucion";


            SqlCommand command = new SqlCommand(consulta, sqlConnection);
            command.Parameters.AddWithValue("@id_ejecucion",ejecucion.idEjecucion);
            command.Parameters.AddWithValue("@id_estado", ejecucion.idestado.idEstado);
            command.Parameters.AddWithValue("@ano_periodo", ejecucion.anoPeriodo);
            command.Parameters.AddWithValue("@id_proyecto", ejecucion.idProyecto);
            command.Parameters.AddWithValue("@monto", ejecucion.monto);
            command.Parameters.AddWithValue("@id_tipo_tramite", ejecucion.idTipoTramite.idTramite);
            command.Parameters.AddWithValue("@numero_referencia", ejecucion.numeroReferencia);
            command.Parameters.AddWithValue("@descripcion_tramite_otro", ejecucion.descripcionEjecucionOtro);


            sqlConnection.Open();
            command.ExecuteReader();
            sqlConnection.Close();
        }
        /// <summary>
        /// Consulta Monto disponible
        /// </summary>
        /// <param name="idPartida"></param>
        ///  <param name="idPresupuestoEgreso"></param>
        public double consultarMontoDiponible(string idPartida,string idPresupuestoEgreso)
        {
            Double monto=0;
            SqlConnection sqlConnection = conexion.conexionPEP();

            String consulta = @"select (
                                         select SUM(monto) as monto from Presupuesto_Egreso_Partida
                                         where id_partida=@idPartida and id_presupuesto_egreso= @idPresupuestoEgreso and id_estado_presupuesto=2 ) - ISNULL((
                                         select SUM(EMPE.monto) as monto_ejecutado from EjecucionMontoPartidasElegidas EMPE where EMPE.id_partida=@idPartida 
                                         and EMPE.id_ejecucion  =( (select E.id_ejecucion from Ejecucion E where E.id_ejecucion=EMPE.id_ejecucion and E.id_estado=2 ))),0)as montoDisponible";

            SqlCommand command = new SqlCommand(consulta, sqlConnection);

            command.Parameters.AddWithValue("@idPartida",Convert.ToInt32(idPartida));
            command.Parameters.AddWithValue("@idPresupuestoEgreso", Convert.ToInt32(idPresupuestoEgreso));

            SqlDataReader reader;
            sqlConnection.Open();
            reader = command.ExecuteReader();
            while (reader.Read())
            {
               monto = Convert.ToDouble(reader["montoDisponible"].ToString());
            }
            sqlConnection.Close();
            return monto;
        }




        /// <summary>
        /// Consulta Ejecucion
        /// </summary>
        /// <param name="Periodo"></param>
        ///  <param name="Proyecto"></param>
        public List<Ejecucion> consultaEjecucion(string Periodo, string Proyecto)
        {
            List<Ejecucion> listaEjecucion = new List<Ejecucion>();
            SqlConnection sqlConnection = conexion.conexionPEP();

            String consulta = @"select descripcion_estado,monto,numero_referencia,nombre_tramite,E.id_ejecucion,T.id_tramite,descripcion_tramite_otro, E.realizado_por,E.fecha
                                      from EstadoEjecucion Es,Ejecucion E,Tipos_tramite T
                                      where E.id_proyecto=@idProyecto and E.ano_periodo=@Periodo and E.id_tipo_tramite= T.id_tramite and E.id_estado= Es.id_estado order by  id_ejecucion desc";

            SqlCommand command = new SqlCommand(consulta, sqlConnection);

            command.Parameters.AddWithValue("@Periodo", Convert.ToInt32(Periodo));
            command.Parameters.AddWithValue("@idProyecto", Convert.ToInt32(Proyecto));

            SqlDataReader reader;
            sqlConnection.Open();
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                EstadoEjecucion estadoEjecucion = new EstadoEjecucion();
                TipoTramite tipoTramite = new TipoTramite();
                Ejecucion ejecucion = new Ejecucion();
                ejecucion.idEjecucion= Convert.ToInt32(reader["id_ejecucion"].ToString());
                ejecucion.monto = Convert.ToInt32(reader["monto"].ToString());
                ejecucion.numeroReferencia = Convert.ToString(reader["numero_referencia"].ToString());
                tipoTramite.nombreTramite = Convert.ToString(reader["nombre_tramite"].ToString());
                tipoTramite.idTramite = Convert.ToInt32(reader["id_tramite"].ToString());
                ejecucion.idTipoTramite = tipoTramite;
                estadoEjecucion.descripcion= Convert.ToString(reader["descripcion_estado"].ToString());
                ejecucion.idestado = estadoEjecucion;
                ejecucion.descripcionEjecucionOtro= Convert.ToString(reader["descripcion_tramite_otro"].ToString());
                ejecucion.realizadoPor = reader["realizado_por"].ToString();
                ejecucion.fecha = Convert.ToDateTime(reader["fecha"].ToString());
                listaEjecucion.Add(ejecucion);
            }
            sqlConnection.Close();
            return listaEjecucion;
        }
        /// <summary>
        /// Consultar una EjecucionUnidad
        /// </summary>
        ///  <param name="idEjecucion"></param>
        public List<Unidad> ConsultarEjecucionUnidad(int idEjecucion )
        {

            List<Unidad> listaUnidad = new List<Unidad>();
            SqlConnection sqlConnection = conexion.conexionPEP();

            String consulta = @"select numero_referencia,id_unidad,nombre_unidad,id_ejecucion
                                            from Unidad_ejecucion
                                            where id_ejecucion=@idEjecucion";

            SqlCommand command = new SqlCommand(consulta, sqlConnection);

            command.Parameters.AddWithValue("@idEjecucion", idEjecucion);

            SqlDataReader reader;
            sqlConnection.Open();
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                Unidad unidad = new Unidad();
                unidad.idUnidad= Convert.ToInt32(reader["id_unidad"].ToString());
                unidad.nombreUnidad= Convert.ToString(reader["nombre_unidad"].ToString());
                listaUnidad.Add(unidad);
            }
            
                sqlConnection.Close();
            return listaUnidad;
        }

        /// <summary>
        /// Consultar una PartidaEjecucion
        /// </summary>
        /// <param name="partida">partida</param>
       
        public List<Partida> ConsultarEjecucionPartidas(int idEjecucion)
        {
            List<Partida> listaPartida = new List<Partida>();
            SqlConnection sqlConnection = conexion.conexionPEP();

            String consulta = @"select P.numero_referencia,P.id_partida,P.numero_partida,P.descripcion_partida,P.id_ejecucion,U.Id_unidad
                                            from Partida_ejecucion P,Unidad_ejecucion U
                                            where P.id_ejecucion=@idEjecucion and u.id_ejecucion=P.id_ejecucion";

            SqlCommand command = new SqlCommand(consulta, sqlConnection);

            command.Parameters.AddWithValue("@idEjecucion", Convert.ToInt32(idEjecucion));

            SqlDataReader reader;
            sqlConnection.Open();
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                Partida partida = new Partida();
                partida.idPartida = Convert.ToInt32(reader["id_partida"].ToString());
                partida.idUnidad = Convert.ToInt32(reader["id_unidad"].ToString());
                partida.numeroPartida = Convert.ToString(reader["numero_partida"].ToString());
                partida.descripcionPartida = Convert.ToString(reader["descripcion_partida"].ToString());

                List<Partida> listaTemp = listaPartida.Where(partidaBD => partidaBD.idPartida == partida.idPartida).ToList();

                if (listaTemp.Count == 0)
                {

                    listaPartida.Add(partida);
                }
            }
           
            sqlConnection.Close();

            return listaPartida;
        }

        /// <summary>
        /// Consultar EjecucionMontoPartidaElegida
        /// </summary>
        /// <param name="idEjecucion">PartidaUnidad</param>
       
        public List<PartidaUnidad> ConsultarEjecucionMontoPartidaElegida( int idEjecucion)
        {
            List<PartidaUnidad> listapartidaUnidad = new List<PartidaUnidad>();
            SqlConnection sqlConnection = conexion.conexionPEP();

            String consulta = @"select  E.numero_referencia,E.id_partida,E.id_unidad,E.monto,E.monto_disponible,E.numero_partida,E.id_ejecucion , U.nombre_unidad
                                           from EjecucionMontoPartidasElegidas E, Unidad U
										   where U.id_unidad = E.id_unidad
                                           and id_ejecucion=@idEjecucion";

            SqlCommand command = new SqlCommand(consulta, sqlConnection);

            command.Parameters.AddWithValue("@idEjecucion", Convert.ToInt32(idEjecucion));

            SqlDataReader reader;
            sqlConnection.Open();
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                PartidaUnidad partidaUnidad = new PartidaUnidad();
                partidaUnidad.IdPartida = Convert.ToInt32(reader["id_partida"].ToString());
                partidaUnidad.IdUnidad = Convert.ToInt32(reader["id_unidad"].ToString());
                partidaUnidad.Monto = Convert.ToDouble(reader["monto"].ToString());
                partidaUnidad.MontoDisponible = Convert.ToDouble(reader["monto_disponible"].ToString());
                partidaUnidad.NumeroPartida = Convert.ToString(reader["numero_partida"].ToString());
                partidaUnidad.nombreUnidad = reader["nombre_unidad"].ToString();
                listapartidaUnidad.Add(partidaUnidad);
            }

            sqlConnection.Close();
            return listapartidaUnidad;
        }
        /// <summary>
        /// Eliminar Ejecucion
        /// </summary>
        /// <param name="idEjecucion">ejecucion</param>
       
        public void eliminarEjecucion(int idEjecucion)
        {
            SqlConnection sqlConnection = conexion.conexionPEP();

            String consulta = @"delete Ejecucion 
                                 where id_ejecucion=@id_ejecucion";


            SqlCommand command = new SqlCommand(consulta, sqlConnection);
            command.Parameters.AddWithValue("@id_ejecucion",idEjecucion);
            


            sqlConnection.Open();
            command.ExecuteReader();
            sqlConnection.Close();
        }

        /// <summary>
        /// Consultar EjecucionMontoPartidaElegida
        /// </summary>
        /// <param name="idEjecucion">PartidaUnidad</param>

        public int ConsultarEjecucionEstado(int idEjecucion)
        {
 
            SqlConnection sqlConnection = conexion.conexionPEP();

            String consulta = @"select  id_estado 
                                           from Ejecucion
                                           where id_ejecucion=@idEjecucion";

            SqlCommand command = new SqlCommand(consulta, sqlConnection);

            command.Parameters.AddWithValue("@idEjecucion", Convert.ToInt32(idEjecucion));

            SqlDataReader reader;
            sqlConnection.Open();
            reader = command.ExecuteReader();
            int idEstado=0;
            while (reader.Read())
            {
                idEstado = Convert.ToInt32(reader["id_estado"].ToString());

            }
            
              
           

            sqlConnection.Close();
            return idEstado;
        }


    }
}

