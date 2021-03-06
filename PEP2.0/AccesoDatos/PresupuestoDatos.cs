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
    /// Adrián Serrano
    /// 22/may/2019
    /// Clase para administrar el CRUD para los presupuestos
    /// </summary>

    public class PresupuestoDatos
    {
       // private ConexionDatos conexion = new ConexionDatos();

       // #region PRESUPUESTO DE INGRESO

       // /// <summary>
       // /// Inserta el presupuesto de ingreso
       // /// </summary>
       // /// <param name="presupuestoIngreso">Presupuesto de ingreso a insertar</param>
       // public int InsertarPresupuestoIngreso(PresupuestoIngreso presupuestoIngreso)
       // {
       //     SqlConnection sqlConnection = conexion.conexionPEP();
       //     int respuesta = 0;
       //     sqlConnection.Open();

       //     try
       //     {
       //         SqlCommand sqlCommand = new SqlCommand("insert into Presupuesto_Ingreso(id_estado_presup_ingreso, monto, es_inicial, id_proyecto) " +
       //             "output INSERTED.id_presupuesto_ingreso values(@idEstadoPresupIngreso, @monto_, @es_inicial_, @id_proyecto_);", sqlConnection);
       //         //El estado por defecto es false=Guardado, mas tarde se cambiara a Aprobado
       //         sqlCommand.Parameters.AddWithValue("@idEstadoPresupIngreso", presupuestoIngreso.estadoPresupIngreso.idEstadoPresupIngreso);
       //         sqlCommand.Parameters.AddWithValue("@monto_", presupuestoIngreso.monto);
       //         sqlCommand.Parameters.AddWithValue("@es_inicial_", presupuestoIngreso.esInicial);
       //         sqlCommand.Parameters.AddWithValue("@id_proyecto_", presupuestoIngreso.proyecto.idProyecto);

       //         respuesta = (int)sqlCommand.ExecuteScalar();
       //     }
       //     catch (SqlException ex)
       //     {
       //         //Utilidades.ErrorBitacora(ex.Message, "Error al insertar el periodo");
       //     }

       //     sqlConnection.Close();

       //     return respuesta;
       // }

       // /// <summary>
       // /// Aprueba el presupuesto dado
       // /// </summary>
       // /// <param name="idPresupuesto">Id del Presupuesto</param>
       // public int AprobarPresupuestoIngreso(int idPresupuesto)
       // {
       //     SqlConnection sqlConnection = conexion.conexionPEP();
       //     int respuesta = 0;
       //     sqlConnection.Open();

       //     try
       //     {
       //         SqlCommand sqlCommand = new SqlCommand("update Presupuesto_Ingreso set id_estado_presup_ingreso=2 output INSERTED.id_presupuesto_ingreso where id_presupuesto_ingreso=@id_presupuesto_ingreso_;", sqlConnection);
       //         sqlCommand.Parameters.AddWithValue("@id_presupuesto_ingreso_", idPresupuesto);

       //         respuesta = (int)sqlCommand.ExecuteScalar();
       //     }
       //     catch (SqlException ex)
       //     {
       //         //Utilidades.ErrorBitacora(ex.Message, "");
       //     }

       //     sqlConnection.Close();

       //     return respuesta;
       // }

       // /// <summary>
       // /// Obtiene los presupuestos por proyectos
       // /// </summary>
       // /// <returns>Retorna una lista <code>LinkedList<PresupuestoIngreso></code> que contiene todos los presupuestos segun el proyecto</returns>
       // public LinkedList<PresupuestoIngreso> ObtenerPorProyecto(int idProyecto)
       // {
       //     SqlConnection sqlConnection = conexion.conexionPEP();
       //     LinkedList<PresupuestoIngreso> presupuestoIngresos = new LinkedList<PresupuestoIngreso>();

       //     SqlCommand sqlCommand = new SqlCommand("SELECT PI.id_presupuesto_ingreso, PI.id_estado_presup_ingreso, PI.monto, PI.es_inicial, PI.id_proyecto, EPI.desc_estado FROM Presupuesto_Ingreso PI, Estado_presup_ingreso EPI where id_proyecto=@id_proyecto_ and EPI.id_estado_presup_ingreso = PI.id_estado_presup_ingreso;", sqlConnection);
       //     sqlCommand.Parameters.AddWithValue("@id_proyecto_", idProyecto);

       //     SqlDataReader reader;
       //     sqlConnection.Open();
       //     reader = sqlCommand.ExecuteReader();

       //     while (reader.Read())
       //     {
       //         PresupuestoIngreso presupuestoIngreso = new PresupuestoIngreso();
       //         EstadoPresupIngreso estadoPresupIngreso = new EstadoPresupIngreso();
       //         estadoPresupIngreso.idEstadoPresupIngreso = Convert.ToInt32(reader["id_estado_presup_ingreso"].ToString());
       //         estadoPresupIngreso.descEstado = reader["desc_estado"].ToString();

       //         presupuestoIngreso.idPresupuestoIngreso = Convert.ToInt32(reader["id_presupuesto_ingreso"].ToString());
       //         presupuestoIngreso.estadoPresupIngreso = estadoPresupIngreso;
       //         presupuestoIngreso.monto = Convert.ToDouble(reader["monto"].ToString());
       //         presupuestoIngreso.esInicial = Convert.ToBoolean(reader["es_inicial"].ToString());

       //         presupuestoIngreso.proyecto = new Proyectos();
       //         presupuestoIngreso.proyecto.idProyecto = Convert.ToInt32(reader["id_proyecto"].ToString());

       //         presupuestoIngresos.AddLast(presupuestoIngreso);
       //     }

       //     sqlConnection.Close();

       //     return presupuestoIngresos;
       // }

       // /// <summary>
       // /// Obtiene el presupuesto por ID
       // /// </summary>
       // /// <returns>Retorna un presupuesto de ingreso</returns>
       // public PresupuestoIngreso ObtenerPorId(int idPresupuesto)
       // {
       //     SqlConnection sqlConnection = conexion.conexionPEP();

       //     SqlCommand sqlCommand = new SqlCommand("SELECT PI.id_presupuesto_ingreso, PI.estado, PI.monto, PI.es_inicial, PI.id_proyecto,EPI.desc_estado FROM Presupuesto_Ingreso PI, Estado_presup_ingreso EPI where id_presupuesto_ingreso=@id_presupuesto_ingreso_ and EPI.id_estado_presup_ingreso = PI.id_estado_presup_ingreso;", sqlConnection);
       //     sqlCommand.Parameters.AddWithValue("@id_presupuesto_ingreso_", idPresupuesto);

       //     SqlDataReader reader;
       //     sqlConnection.Open();
       //     reader = sqlCommand.ExecuteReader();

       //     PresupuestoIngreso presupuestoIngreso = new PresupuestoIngreso();

       //     if (reader.Read())
       //     {
       //         EstadoPresupIngreso estadoPresupIngreso = new EstadoPresupIngreso();
       //         estadoPresupIngreso.idEstadoPresupIngreso = Convert.ToInt32(reader["id_estado_presup_ingreso"].ToString());
       //         estadoPresupIngreso.descEstado = reader["desc_estado"].ToString();

       //         presupuestoIngreso.idPresupuestoIngreso = Convert.ToInt32(reader["id_presupuesto_ingreso"].ToString());
       //         presupuestoIngreso.estadoPresupIngreso = estadoPresupIngreso;
       //         presupuestoIngreso.monto = Convert.ToDouble(reader["monto"].ToString());
       //         presupuestoIngreso.esInicial = Convert.ToBoolean(reader["es_inicial"].ToString());

       //         presupuestoIngreso.proyecto = new Proyectos();
       //         presupuestoIngreso.proyecto.idProyecto = Convert.ToInt32(reader["id_proyecto"].ToString());
       //     }

       //     sqlConnection.Close();

       //     return presupuestoIngreso;
       // }

       // /// <summary>
       // /// Eliminar un presupuesto de ingreso
       // /// </summary>
       // /// <param name="idPresupuesto">Id del presupuesto a eliminar</param>
       // public int EliminarPresupuestoIngreso(int idPresupuesto)
       // {
       //     SqlConnection sqlConnection = conexion.conexionPEP();
       //     sqlConnection.Open();
       //     int respuesta = 0;

       //     try
       //     {
       //         SqlCommand sqlCommand = new SqlCommand("delete from Presupuesto_Ingreso output deleted.id_presupuesto_ingreso where id_presupuesto_ingreso=@id_presupuesto_ingreso_;", sqlConnection);
       //         sqlCommand.Parameters.AddWithValue("@id_presupuesto_ingreso_", idPresupuesto);

       //         respuesta = (int)sqlCommand.ExecuteScalar();
       //     }
       //     catch (SqlException ex)
       //     {
       //         //Utilidades.ErrorBitacora(ex.Message, "");
       //     }

       //     sqlConnection.Close();

       //     return respuesta;
       // }

       // /// <summary>
       // /// Leonardo Carrion
       // /// 27/sep/2019
       // /// Efecto: devuelve lista de presupuestos de ingresos segun el proyecto ingresado
       // /// Requiere: proyecto a consultar
       // /// Modifica: -
       // /// Devuelve: lista de presupuestos de ingresos
       // /// </summary>
       // /// <param name="proyecto"></param>
       // /// <returns></returns>
       // public List<PresupuestoIngreso> getPresupuestosIngresosPorProyecto(Proyectos proyecto)
       // {
       //     SqlConnection sqlConnection = conexion.conexionPEP();
       //     List<PresupuestoIngreso> presupuestoIngresos = new List<PresupuestoIngreso>();

       //     SqlCommand sqlCommand = new SqlCommand("SELECT PI.id_presupuesto_ingreso, PI.id_estado_presup_ingreso, PI.monto, PI.es_inicial, PI.id_proyecto, EPI.desc_estado FROM Presupuesto_Ingreso PI, Estado_presup_ingreso EPI where id_proyecto=@id_proyecto_ and EPI.id_estado_presup_ingreso = PI.id_estado_presup_ingreso;", sqlConnection);
       //     sqlCommand.Parameters.AddWithValue("@id_proyecto_", proyecto.idProyecto);

       //     SqlDataReader reader;
       //     sqlConnection.Open();
       //     reader = sqlCommand.ExecuteReader();

       //     while (reader.Read())
       //     {
       //         PresupuestoIngreso presupuestoIngreso = new PresupuestoIngreso();
       //         EstadoPresupIngreso estadoPresupIngreso = new EstadoPresupIngreso();
       //         estadoPresupIngreso.idEstadoPresupIngreso = Convert.ToInt32(reader["id_estado_presup_ingreso"].ToString());
       //         estadoPresupIngreso.descEstado = reader["desc_estado"].ToString();

       //         presupuestoIngreso.idPresupuestoIngreso = Convert.ToInt32(reader["id_presupuesto_ingreso"].ToString());
       //         presupuestoIngreso.estadoPresupIngreso = estadoPresupIngreso;
       //         presupuestoIngreso.monto = Convert.ToDouble(reader["monto"].ToString());
       //         presupuestoIngreso.esInicial = Convert.ToBoolean(reader["es_inicial"].ToString());

       //         presupuestoIngreso.proyecto = new Proyectos();
       //         presupuestoIngreso.proyecto.idProyecto = Convert.ToInt32(reader["id_proyecto"].ToString());

       //         presupuestoIngresos.Add(presupuestoIngreso);
       //     }

       //     sqlConnection.Close();

       //     return presupuestoIngresos;
       // }

       // /// <summary>
       // /// Leonardo Carrion
       // /// 01/oct/2019
       // /// Efecto: actualiza dado de monto del presupuesto de ingreso 
       // /// Requiere: presupuesto de ingreso a modificar
       // /// Modifica: dato de monto del presupuesto ingreso
       // /// Devuelve: -
       // /// </summary>
       // /// <param name="presupuestoIngreso"></param>
       // /// <returns></returns>
       // public void actualizarPresupuestoIngreso(PresupuestoIngreso presupuestoIngreso)
       // {
       //     SqlConnection sqlConnection = conexion.conexionPEP();

       //     SqlCommand sqlCommand = new SqlCommand(@"update Presupuesto_Ingreso set monto=@monto where id_presupuesto_ingreso = @idPresupuestoIngreso", sqlConnection);
       //     sqlCommand.Parameters.AddWithValue("@monto", presupuestoIngreso.monto);
       //     sqlCommand.Parameters.AddWithValue("@idPresupuestoIngreso", presupuestoIngreso.idPresupuestoIngreso);

       //     sqlConnection.Open();
       //     sqlCommand.ExecuteReader();
       //     sqlConnection.Close();
       // }

       // /// <summary>
       // /// Leonardo Carrion
       // /// 02/oct/2019
       // /// Efecto: actualiza dado de estado del presupuesto de ingreso 
       // /// Requiere: presupuesto de ingreso a modificar
       // /// Modifica: dato de estado del presupuesto ingreso
       // /// Devuelve: -
       // /// </summary>
       // /// <param name="presupuestoIngreso"></param>
       // /// <returns></returns>
       // public void actualizarEstadoPresupuestoIngreso(PresupuestoIngreso presupuestoIngreso)
       // {
       //     SqlConnection sqlConnection = conexion.conexionPEP();

       //     SqlCommand sqlCommand = new SqlCommand(@"update Presupuesto_Ingreso set id_estado_presup_ingreso=@idEstadoPresupIngreso where id_presupuesto_ingreso = @idPresupuestoIngreso", sqlConnection);
       //     sqlCommand.Parameters.AddWithValue("@idEstadoPresupIngreso", presupuestoIngreso.estadoPresupIngreso.idEstadoPresupIngreso);
       //     sqlCommand.Parameters.AddWithValue("@idPresupuestoIngreso", presupuestoIngreso.idPresupuestoIngreso);

       //     sqlConnection.Open();
       //     sqlCommand.ExecuteReader();
       //     sqlConnection.Close();
       // }

       // #endregion PRESUPUESTO DE INGRESO

       // #region PRESUPUESTO DE EGRESO

       // /// <summary>
       // /// Leonardo Carrion
       // /// 04/oct/2019
       // /// Efecto: actualiza dado de plan estrategico del presupuesto de egreso 
       // /// Requiere: presupuesto de egreso a modificar
       // /// Modifica: dato de plan estrategico del presupuesto egreso
       // /// Devuelve: -
       // /// </summary>
       // /// <param name="presupuestoEgreso"></param>
       // /// <returns></returns>
       // public void actualizarPlanEstrategicoPresupuestoEgreso(PresupuestoEgreso presupuestoEgreso)
       // {
       //     SqlConnection sqlConnection = conexion.conexionPEP();

       //     SqlCommand sqlCommand = new SqlCommand(@"update Presupuesto_Egreso set plan_estrategico_operacional=@planEstrategicoOperacional where id_presupuesto_egreso = @idPresupuestoEgreso", sqlConnection);
       //     sqlCommand.Parameters.AddWithValue("@planEstrategicoOperacional", presupuestoEgreso.planEstrategicoOperacional);
       //     sqlCommand.Parameters.AddWithValue("@idPresupuestoEgreso", presupuestoEgreso.idPresupuestoEgreso);

       //     sqlConnection.Open();
       //     sqlCommand.ExecuteReader();
       //     sqlConnection.Close();
       // }

       // /// <summary>
       // /// Inserta un nuevo presupuesto de egreso
       // /// </summary>
       // /// <param name="presupuestoEgreso">Presupuesto de Egreso</param>
       // public int InsertarPresupuestoEgreso(PresupuestoEgreso presupuestoEgreso)
       // {
       //     SqlConnection sqlConnection = conexion.conexionPEP();
       //     int idPresupuestoEgreso = 0;
       //     sqlConnection.Open();

       //     try
       //     {
       //         SqlCommand sqlCommand = new SqlCommand("insert into Presupuesto_Egreso(id_unidad, id_estado,  plan_estrategico_operacional, montoTotal) " +
       //                 "output INSERTED.id_presupuesto_egreso values(@id_unidad_,@idEstado , @plan_estrategico_operacional_, @montoTotal_);", sqlConnection);
       //         //El estado por defecto es false=Pendiente, mas tarde se cambiara a Aprobado
       //         sqlCommand.Parameters.AddWithValue("@id_unidad_", presupuestoEgreso.idUnidad);
       //         sqlCommand.Parameters.AddWithValue("@idEstado", presupuestoEgreso.estadoPresupuesto.idEstadoPresupuesto);
       //         sqlCommand.Parameters.AddWithValue("@plan_estrategico_operacional_", presupuestoEgreso.planEstrategicoOperacional);
       //         sqlCommand.Parameters.AddWithValue("@montoTotal_", presupuestoEgreso.montoTotal);

       //         idPresupuestoEgreso = (int)sqlCommand.ExecuteScalar();

       //     }
       //     catch (SqlException ex)
       //     {
       //         //Utilidades.ErrorBitacora(ex.Message, "Error al insertar el periodo");
       //     }

       //     sqlConnection.Close();

       //     return idPresupuestoEgreso;
       // }

       // /// <summary>
       // /// Asocia cada una de las partidas con su respectivo monto
       // /// </summary>
       // /// <param name="presupuestoEgresoPartida">Presupuesto de egreso de partidas relacionadas con el presupuesto de egreso</param>
       // public void InsertarPresupuestoEgresoPartida(PresupuestoEgresoPartida presupuestoEgresoP)
       // {
       //     SqlConnection sqlConnection = conexion.conexionPEP();
       //     sqlConnection.Open();

       //     try
       //     {

       //         SqlCommand sqlCommand = new SqlCommand("insert into Presupuesto_Egreso_Partida(id_presupuesto_egreso, id_partida, monto, descripcion) " +
       //         "values(@id_presupuesto_egreso_, @id_partida_, @monto_, @descripcion_);", sqlConnection);
       //         sqlCommand.Parameters.AddWithValue("@id_presupuesto_egreso_", presupuestoEgresoP.idPresupuestoEgreso);
       //         sqlCommand.Parameters.AddWithValue("@id_partida_", presupuestoEgresoP.partida.idPartida);
       //         sqlCommand.Parameters.AddWithValue("@monto_", presupuestoEgresoP.monto);
       //         sqlCommand.Parameters.AddWithValue("@descripcion_", presupuestoEgresoP.descripcion);

       //         sqlCommand.ExecuteScalar();


       //     }
       //     catch (SqlException ex)
       //     {
       //         //Utilidades.ErrorBitacora(ex.Message, "Error al insertar el periodo");
       //     }

       //     sqlConnection.Close();
       // }

       // /// <summary>
       // /// Josseline M
       // /// Filtra los presupuestos egresos de acuerdo con el numero de partida 
       // /// </summary>
       // /// <param name="presupuestoEgresoPartidaF"></param>
       // /// <returns></returns>
       // public LinkedList<PresupuestoEgresoPartida> presupuestoEgresoPartidasPorPresupuesto(PresupuestoEgresoPartida presupuestoEgresoPartidaF)
       // {
       //     SqlConnection sqlConnectionPresupuestoEgreso = conexion.conexionPEP();
       //     SqlConnection sqlConnectionPresupuestoEgresoPartida = conexion.conexionPEP();

       //     SqlCommand sqlCommandPresupuestoEgreso = new SqlCommand("SELECT id_presupuesto_egreso,id_partida,monto, descripcion FROM Presupuesto_Egreso_Partida Where id_presupuesto_egreso = @id_partida; ", sqlConnectionPresupuestoEgreso);

       //     sqlCommandPresupuestoEgreso.Parameters.AddWithValue("@id_partida", presupuestoEgresoPartidaF.partida.idPartida);

       //     SqlDataReader readerPresupuestoEgreso;
       //     sqlConnectionPresupuestoEgreso.Open();
       //     readerPresupuestoEgreso = sqlCommandPresupuestoEgreso.ExecuteReader();

       //     LinkedList<PresupuestoEgresoPartida> presupuestoEgresosPartidaL = new LinkedList<PresupuestoEgresoPartida>();

       //     while (readerPresupuestoEgreso.Read())
       //     {

       //         PresupuestoEgresoPartida presupuestoEgresoPartida = new PresupuestoEgresoPartida();
       //         presupuestoEgresoPartida.idPresupuestoEgreso = Convert.ToInt32(readerPresupuestoEgreso["id_presupuesto_egreso"].ToString());
       //         presupuestoEgresoPartida.partida.idPartida = Convert.ToInt32(readerPresupuestoEgreso["id_partida"].ToString());
       //         presupuestoEgresoPartida.monto = Convert.ToDouble(readerPresupuestoEgreso["monto"].ToString());
       //         presupuestoEgresoPartida.descripcion= readerPresupuestoEgreso["descripcion"].ToString();
       //         presupuestoEgresosPartidaL.AddLast(presupuestoEgresoPartida);
       //     }


       //     sqlConnectionPresupuestoEgresoPartida.Close();


       //     return presupuestoEgresosPartidaL;
       // }
             
       // /// <summary>
       // /// Josseline M 
       // /// Este metodo retorna una lista de presuestos egresos destinados por unidad
       // /// </summary>
       // /// <param name="idUnidad"></param>
       // /// <returns></returns>
       // public List<PresupuestoEgreso> ObtenerPorUnidadEgresos(int idUnidad)
       // {
       //     SqlConnection sqlConnectionPresupuestoEgreso = conexion.conexionPEP();
       //     SqlConnection sqlConnectionPresupuestoEgresoPartida = conexion.conexionPEP();

       //     SqlCommand sqlCommandPresupuestoEgreso = new SqlCommand("SELECT id_presupuesto_egreso, id_unidad, montoTotal, plan_estrategico_operacional FROM Presupuesto_Egreso where id_unidad=@id_unidad_;", sqlConnectionPresupuestoEgreso);
       //     sqlCommandPresupuestoEgreso.Parameters.AddWithValue("@id_unidad_", idUnidad);

       //     SqlDataReader readerPresupuestoEgreso;
       //     sqlConnectionPresupuestoEgreso.Open();
       //     readerPresupuestoEgreso = sqlCommandPresupuestoEgreso.ExecuteReader();

       //     List<PresupuestoEgreso> presupuestoEgresos = new List<PresupuestoEgreso>();

       //     while (readerPresupuestoEgreso.Read())
       //     {
       //         PresupuestoEgreso presupuestoEgreso = new PresupuestoEgreso();

       //         presupuestoEgreso.idPresupuestoEgreso = Convert.ToInt32(readerPresupuestoEgreso["id_presupuesto_egreso"].ToString());
       //         presupuestoEgreso.idUnidad = Convert.ToInt32(readerPresupuestoEgreso["id_unidad"].ToString());
       //         presupuestoEgreso.planEstrategicoOperacional = readerPresupuestoEgreso["plan_estrategico_operacional"].ToString();
       //         presupuestoEgreso.montoTotal = Convert.ToDouble(readerPresupuestoEgreso["montoTotal"].ToString());

       //         presupuestoEgreso.presupuestoEgresoPartidas = new LinkedList<PresupuestoEgresoPartida>();
       //         SqlCommand sqlCommandPresupuestoEgresoPartidas = new SqlCommand("SELECT PEP.*, P.descripcion_partida, P.numero_partida FROM Presupuesto_Egreso_Partida PEP, Partida P where id_presupuesto_egreso=@id_presupuesto_egreso_ and P.id_partida = PEP.id_partida;", sqlConnectionPresupuestoEgresoPartida);
       //         sqlCommandPresupuestoEgresoPartidas.Parameters.AddWithValue("@id_presupuesto_egreso_", presupuestoEgreso.idPresupuestoEgreso);

       //         SqlDataReader readerPresupuestoEgresoPartidas;
       //         sqlConnectionPresupuestoEgresoPartida.Open();
       //         readerPresupuestoEgresoPartidas = sqlCommandPresupuestoEgresoPartidas.ExecuteReader();

       //         while (readerPresupuestoEgresoPartidas.Read())
       //         {
       //             PresupuestoEgresoPartida presupuestoEgresoPartida = new PresupuestoEgresoPartida();
       //             Partida partida = new Partida();
       //             partida.idPartida = Convert.ToInt32(readerPresupuestoEgresoPartidas["id_partida"].ToString());
       //             partida.descripcionPartida = readerPresupuestoEgresoPartidas["descripcion_partida"].ToString();
       //             partida.numeroPartida = readerPresupuestoEgresoPartidas["numero_partida"].ToString();

       //             presupuestoEgresoPartida.idPresupuestoEgreso = Convert.ToInt32(readerPresupuestoEgresoPartidas["id_presupuesto_egreso"].ToString());
       //             presupuestoEgresoPartida.monto = Convert.ToDouble(readerPresupuestoEgresoPartidas["monto"].ToString());


       //             presupuestoEgreso.presupuestoEgresoPartidas.AddLast(presupuestoEgresoPartida);
       //         }

       //         presupuestoEgresos.Add(presupuestoEgreso);
       //         sqlConnectionPresupuestoEgresoPartida.Close();

       //     }
       //     sqlConnectionPresupuestoEgreso.Close();

       //     return presupuestoEgresos;
       // }

       // /// <summary>
       // /// Obtiene los presupuesto por unidad
       // /// </summary>
       // /// <returns>Retorna varios presupuesto de egreso</returns>
       // public LinkedList<PresupuestoEgreso> ObtenerPorUnidad(int idUnidad)
       // {
       //     SqlConnection sqlConnectionPresupuestoEgreso = conexion.conexionPEP();
       //     SqlConnection sqlConnectionPresupuestoEgresoPartida = conexion.conexionPEP();

       //     SqlCommand sqlCommandPresupuestoEgreso = new SqlCommand("SELECT id_presupuesto_egreso, id_unidad, montoTotal, plan_estrategico_operacional FROM Presupuesto_Egreso where id_unidad=@id_unidad_;", sqlConnectionPresupuestoEgreso);
       //     sqlCommandPresupuestoEgreso.Parameters.AddWithValue("@id_unidad_", idUnidad);

       //     SqlDataReader readerPresupuestoEgreso;
       //     sqlConnectionPresupuestoEgreso.Open();
       //     readerPresupuestoEgreso = sqlCommandPresupuestoEgreso.ExecuteReader();

       //     LinkedList<PresupuestoEgreso> presupuestoEgresos = new LinkedList<PresupuestoEgreso>();

       //     while (readerPresupuestoEgreso.Read())
       //     {
       //         PresupuestoEgreso presupuestoEgreso = new PresupuestoEgreso();

       //         presupuestoEgreso.idPresupuestoEgreso = Convert.ToInt32(readerPresupuestoEgreso["id_presupuesto_egreso"].ToString());
       //         presupuestoEgreso.idUnidad = Convert.ToInt32(readerPresupuestoEgreso["id_unidad"].ToString());
       //         presupuestoEgreso.planEstrategicoOperacional = readerPresupuestoEgreso["plan_estrategico_operacional"].ToString();
       //         presupuestoEgreso.montoTotal = Convert.ToDouble(readerPresupuestoEgreso["montoTotal"].ToString());

       //         presupuestoEgreso.presupuestoEgresoPartidas = new LinkedList<PresupuestoEgresoPartida>();
       //         SqlCommand sqlCommandPresupuestoEgresoPartidas = new SqlCommand("SELECT id_presupuesto_egreso, id_partida, monto FROM Presupuesto_Egreso_Partida where id_presupuesto_egreso=@id_presupuesto_egreso_;", sqlConnectionPresupuestoEgresoPartida);
       //         sqlCommandPresupuestoEgresoPartidas.Parameters.AddWithValue("@id_presupuesto_egreso_", presupuestoEgreso.idPresupuestoEgreso);

       //         SqlDataReader readerPresupuestoEgresoPartidas;
       //         sqlConnectionPresupuestoEgresoPartida.Open();
       //         readerPresupuestoEgresoPartidas = sqlCommandPresupuestoEgresoPartidas.ExecuteReader();

       //         while (readerPresupuestoEgresoPartidas.Read())
       //         {
       //             PresupuestoEgresoPartida presupuestoEgresoPartida = new PresupuestoEgresoPartida();
       //             presupuestoEgresoPartida.idPresupuestoEgreso = Convert.ToInt32(readerPresupuestoEgresoPartidas["id_presupuesto_egreso"].ToString());
       //             presupuestoEgresoPartida.partida.idPartida = Convert.ToInt32(readerPresupuestoEgresoPartidas["id_partida"].ToString());
       //             presupuestoEgresoPartida.monto = Convert.ToDouble(readerPresupuestoEgresoPartidas["monto"].ToString());
                    
       //             presupuestoEgreso.presupuestoEgresoPartidas.AddLast(presupuestoEgresoPartida);
       //         }

       //         presupuestoEgresos.AddLast(presupuestoEgreso);
       //         sqlConnectionPresupuestoEgresoPartida.Close();
       //     }

       //     sqlConnectionPresupuestoEgreso.Close();

       //     return presupuestoEgresos;
       // }
       
       // /// <summary>
       // /// Retorna el presupuesto de los proyecto por proyecto y por unidad
       // /// </summary>
       // /// <param name="idProyecto"></param>
       // /// <returns></returns>
       // public LinkedList<PresupuestoEgreso> ObtenerPresupuestoPorProyecto(int idUnidad,int idProyecto)
       // {
       //     SqlConnection sqlConnectionPresupuestoEgreso = conexion.conexionPEP();
       //     SqlConnection sqlConnectionPresupuestoEgresoPartida = conexion.conexionPEP();

       //     SqlCommand sqlCommandPresupuestoEgreso = new SqlCommand("SELECT id_presupuesto_egreso, Presupuesto_Egreso.id_unidad, montoTotal, plan_estrategico_operacional FROM Presupuesto_Egreso, Unidad where Presupuesto_Egreso.id_unidad=@id_unidad_ and Unidad.id_proyecto=@id_proyecto_; ", sqlConnectionPresupuestoEgreso);
            
       //     sqlCommandPresupuestoEgreso.Parameters.AddWithValue("@id_proyecto_", idProyecto);
       //     sqlCommandPresupuestoEgreso.Parameters.AddWithValue("@id_unidad_", idUnidad);
       //     SqlDataReader readerPresupuestoEgreso;
       //     sqlConnectionPresupuestoEgreso.Open();
       //     readerPresupuestoEgreso = sqlCommandPresupuestoEgreso.ExecuteReader();

       //     LinkedList<PresupuestoEgreso> presupuestoEgresos = new LinkedList<PresupuestoEgreso>();

       //     while (readerPresupuestoEgreso.Read())
       //     {
       //         PresupuestoEgreso presupuestoEgreso = new PresupuestoEgreso();

       //         presupuestoEgreso.idPresupuestoEgreso = Convert.ToInt32(readerPresupuestoEgreso["id_presupuesto_egreso"].ToString());
       //         double monto = ObtenerMontoPartida(presupuestoEgreso.idPresupuestoEgreso);
       //         ActualizarMontoTotalPresupuesto(presupuestoEgreso.idPresupuestoEgreso, monto);
       //         presupuestoEgreso.idUnidad = Convert.ToInt32(readerPresupuestoEgreso["id_unidad"].ToString());
       //         presupuestoEgreso.planEstrategicoOperacional = readerPresupuestoEgreso["plan_estrategico_operacional"].ToString();
              
       //         presupuestoEgreso.montoTotal = Convert.ToDouble(readerPresupuestoEgreso["montoTotal"].ToString());
       //         presupuestoEgreso.descripcion = ObtenerDescripcionesPartida(presupuestoEgreso.idPresupuestoEgreso);

       //         presupuestoEgreso.presupuestoEgresoPartidas = new LinkedList<PresupuestoEgresoPartida>();
       //         SqlCommand sqlCommandPresupuestoEgresoPartidas = new SqlCommand("SELECT id_presupuesto_egreso, id_partida, monto FROM Presupuesto_Egreso_Partida where id_presupuesto_egreso=@id_presupuesto_egreso_;", sqlConnectionPresupuestoEgresoPartida);
       //         sqlCommandPresupuestoEgresoPartidas.Parameters.AddWithValue("@id_presupuesto_egreso_", presupuestoEgreso.idPresupuestoEgreso);

       //         SqlDataReader readerPresupuestoEgresoPartidas;
       //         sqlConnectionPresupuestoEgresoPartida.Open();
       //         readerPresupuestoEgresoPartidas = sqlCommandPresupuestoEgresoPartidas.ExecuteReader();

       //         while (readerPresupuestoEgresoPartidas.Read())
       //         {
       //             PresupuestoEgresoPartida presupuestoEgresoPartida = new PresupuestoEgresoPartida();
       //             presupuestoEgresoPartida.idPresupuestoEgreso = Convert.ToInt32(readerPresupuestoEgresoPartidas["id_presupuesto_egreso"].ToString());
       //             presupuestoEgresoPartida.partida.idPartida = Convert.ToInt32(readerPresupuestoEgresoPartidas["id_partida"].ToString());
       //             presupuestoEgresoPartida.monto = Convert.ToDouble(readerPresupuestoEgresoPartidas["monto"].ToString());

       //             presupuestoEgreso.presupuestoEgresoPartidas.AddLast(presupuestoEgresoPartida);
                   
       //         }

       //         presupuestoEgresos.AddLast(presupuestoEgreso);
       //         sqlConnectionPresupuestoEgresoPartida.Close();
       //     }

       //     sqlConnectionPresupuestoEgreso.Close();

       //     return presupuestoEgresos;
       // }
        
       // /// <summary>
       // /// Este metodo muestra las descripciones existentes para esa partida
       // /// </summary>
       // private string ObtenerDescripcionesPartida(int idPresupuestoE)
       // {
       //     string descricpcion = "";
       //     SqlConnection sqlConnectionPresupuestoEgreso = conexion.conexionPEP();
       //     SqlConnection sqlConnectionPresupuestoEgresoPartida = conexion.conexionPEP();

       //     SqlCommand sqlCommandPresupuestoEgreso = new SqlCommand("SELECT descripcion FROM Presupuesto_Egreso_Partida where id_presupuesto_egreso = @id_presupuesto_egreso", sqlConnectionPresupuestoEgreso);

       //     sqlCommandPresupuestoEgreso.Parameters.AddWithValue("@id_presupuesto_egreso", idPresupuestoE);

       //     SqlDataReader readerPresupuestoEgreso;
       //     sqlConnectionPresupuestoEgreso.Open();
       //     readerPresupuestoEgreso = sqlCommandPresupuestoEgreso.ExecuteReader();
       //     while (readerPresupuestoEgreso.Read())
       //     {
       //         descricpcion +=readerPresupuestoEgreso["descripcion"].ToString()+"<br/>";
               
               
       //     }

       //     sqlConnectionPresupuestoEgreso.Close();
       //     return descricpcion;
       // }
        
       // /// <summary>
       // /// Este metodo suma los montos existentes para esa partida
       // /// </summary>
       // private double ObtenerMontoPartida(int idPresupuestoE)
       // {
       //     double monto = 0;
       //     SqlConnection sqlConnectionPresupuestoEgreso = conexion.conexionPEP();
       //      SqlCommand sqlCommandPresupuestoEgreso = new SqlCommand("SELECT monto FROM Presupuesto_Egreso_Partida where id_presupuesto_egreso = @id_presupuesto_egreso", sqlConnectionPresupuestoEgreso);

       //     sqlCommandPresupuestoEgreso.Parameters.AddWithValue("@id_presupuesto_egreso", idPresupuestoE);

       //     SqlDataReader readerPresupuestoEgreso;
       //     sqlConnectionPresupuestoEgreso.Open();

       //     readerPresupuestoEgreso = sqlCommandPresupuestoEgreso.ExecuteReader();
       //     while (readerPresupuestoEgreso.Read())
       //     {
                
       //         monto += Convert.ToDouble(readerPresupuestoEgreso["monto"]);
             
       //     }

       
       //     sqlConnectionPresupuestoEgreso.Close();
       //     return monto;
           
       // }
       
        
       // /// <summary>
       // /// Josseline M
       // /// Actualiza el monto total de presupuesto egreso a partir de idPresupuestoEgreso
       // /// </summary>
       // /// <param name="idPresupuestoE"></param>
       // /// <param name="monto"></param>
       // private void ActualizarMontoTotalPresupuesto(int idPresupuestoE, double monto)
       // {
       //     SqlConnection sqlConnectionPresupuestoEgreso = conexion.conexionPEP();
       //     SqlCommand sqlCommandPresupuestoEgreso = new SqlCommand("Update Presupuesto_Egreso set montoTotal= @monto where id_presupuesto_egreso = @id_presupuesto_egreso", sqlConnectionPresupuestoEgreso);
       //     sqlCommandPresupuestoEgreso.Parameters.AddWithValue("@id_presupuesto_egreso", idPresupuestoE);
       //     sqlCommandPresupuestoEgreso.Parameters.AddWithValue("@monto", monto);
      
  
       //     sqlConnectionPresupuestoEgreso.Open();
       //     sqlCommandPresupuestoEgreso.ExecuteScalar();


       //     sqlConnectionPresupuestoEgreso.Close();
       // }
       
       // /// <summary>
       // /// Este metodo retorna 1 de ser inferior el monto total del presupuesto egresos en compración al 
       // /// diponible
       // /// </summary>
       // /// <param name="presupuesto"></param>
       // /// <returns></returns>
       // public int AprobarPresupuestoEgresoPorMonto(PresupuestoEgreso presupuesto)
       // {
       //     int esMenor = 0;
       //     double montoDisponible = 0;
       //     double montoTotalPresupuestos = 0;
       //     SqlConnection sqlConnectionPresupuestoEgreso = conexion.conexionPEP();
          
       //     SqlCommand sqlCommandPresupuestoEgreso = new SqlCommand("select monto FROM Presupuesto_Egreso,Unidad,Proyecto, Presupuesto_Ingreso where  Unidad.id_unidad=@id_unidad_ and Unidad.id_proyecto=Proyecto.id_proyecto and Proyecto.id_proyecto=Presupuesto_Ingreso.id_proyecto", sqlConnectionPresupuestoEgreso);

       //     sqlCommandPresupuestoEgreso.Parameters.AddWithValue("@id_unidad_", presupuesto.idUnidad);
       //     SqlDataReader readerPresupuestoEgreso;
       //     sqlConnectionPresupuestoEgreso.Open();
       //     readerPresupuestoEgreso = sqlCommandPresupuestoEgreso.ExecuteReader();


       //     while (readerPresupuestoEgreso.Read())
       //     {

       //         montoDisponible = Convert.ToDouble(readerPresupuestoEgreso["monto"].ToString());

       //     }

       //     montoTotalPresupuestos = ObtenerMontoPartida(presupuesto.idPresupuestoEgreso);

       //     if (montoTotalPresupuestos<=montoDisponible)
       //     {
       //         actualizaEstadoPresupuestoEgreso(presupuesto.idPresupuestoEgreso,1);
              
       //         esMenor = 1;
       //     }

       //     sqlConnectionPresupuestoEgreso.Close();

       //     return esMenor;
       // }

       // /// <summary>
       // /// Josseline M
       // /// Este método actualiza el estado del presupuesto egreso es decir se la asigna 1 si este ha sido aprobado
       // /// o 2 si ha sido guardado pero no aprobado
       // /// </summary>
       // /// <param name="idPresupuestoEgreso"></param>
       // /// <param name="valorEstado"></param>
       // private void actualizaEstadoPresupuestoEgreso(int idPresupuestoEgreso,int valorEstado)
       // {
       //     SqlConnection sqlConnectionPresupuestoEgreso = conexion.conexionPEP();
       //     SqlCommand sqlCommandPresupuestoEgreso = new SqlCommand("UPDATE Presupuesto_Egreso  SET id_estado = @id_estado_ where id_presupuesto_egreso=@id_presupuesto_egreso_", sqlConnectionPresupuestoEgreso);
       //     sqlCommandPresupuestoEgreso.Parameters.AddWithValue("@id_presupuesto_egreso_", idPresupuestoEgreso);
       //     sqlCommandPresupuestoEgreso.Parameters.AddWithValue("@id_estado_", valorEstado);
       //     sqlConnectionPresupuestoEgreso.Open();
       //     sqlCommandPresupuestoEgreso.ExecuteScalar();
       //     sqlConnectionPresupuestoEgreso.Close();


       // }

       // /// <summary>
       // /// Guarda el avance obtenido en el añadimiento de partidas
       // /// </summary>
       // /// <param name="presupuestoE"></param>
       // public void guardarPartidasPresupuestoEgreso(LinkedList<PresupuestoEgreso> presupuestosE)
       // {
       //     foreach (PresupuestoEgreso presupuestoIngresar in presupuestosE)
       //     {
       //         actualizaEstadoPresupuestoEgreso(presupuestoIngresar.idPresupuestoEgreso, 2);
       //     }
           
       // }
       
       // /// <summary>
       ///// Este método se encarga de actualizar las descripciones y montos de las partidas egresos
       ///// </summary>
       ///// <param name="presupuesto"></param>
       // public void editarPresupuestoEgresoPartida(PresupuestoEgresoPartida presupuestoEgresoPartida)
       // {
       //     SqlConnection sqlConnectionPresupuestoEgreso = conexion.conexionPEP();
       //     SqlCommand sqlCommandPresupuestoEgreso = new SqlCommand("UPDATE Presupuesto_Egreso_Partida  SET monto = @monto_, descripcion= @descripcion_ where id_partida=@id_partida_ and id_presupuesto_egreso=@id_presupuesto_egreso_", sqlConnectionPresupuestoEgreso);
       //     sqlCommandPresupuestoEgreso.Parameters.AddWithValue("@id_partida_", presupuestoEgresoPartida.partida.idPartida);
       //     sqlCommandPresupuestoEgreso.Parameters.AddWithValue("@id_presupuesto_egreso_", presupuestoEgresoPartida.idPresupuestoEgreso);
       //     sqlCommandPresupuestoEgreso.Parameters.AddWithValue("@monto_", presupuestoEgresoPartida.monto);
       //     sqlCommandPresupuestoEgreso.Parameters.AddWithValue("@descripcion_", presupuestoEgresoPartida.descripcion);
       //     sqlConnectionPresupuestoEgreso.Open();
       //     sqlCommandPresupuestoEgreso.ExecuteScalar();
       //     sqlConnectionPresupuestoEgreso.Close();

       // }
       
       // /// <summary>
       // /// Este método se encarga de eliminar las partidas egresos
       // /// </summary>
       // /// <param name="presupuesto"></param>
       // public void eliminarPresupuestoEgresoPartida(PresupuestoEgresoPartida presupuestoEgresoPartida)
       // {
       //     SqlConnection sqlConnectionPresupuestoEgreso = conexion.conexionPEP();
       //     SqlCommand sqlCommandPresupuestoEgreso = new SqlCommand("delete Presupuesto_Egreso_Partida where id_partida=@id_partida_ and id_presupuesto_egreso=@id_presupuesto_egreso_", sqlConnectionPresupuestoEgreso);
       //     sqlCommandPresupuestoEgreso.Parameters.AddWithValue("@id_partida_", presupuestoEgresoPartida.partida.idPartida);
       //     sqlCommandPresupuestoEgreso.Parameters.AddWithValue("@id_presupuesto_egreso_", presupuestoEgresoPartida.idPresupuestoEgreso);
       //     sqlConnectionPresupuestoEgreso.Open();
       //     sqlCommandPresupuestoEgreso.ExecuteScalar();
       //     sqlConnectionPresupuestoEgreso.Close();

       // }

       // #endregion PRESUPUESTO DE EGRESO
    }
}
