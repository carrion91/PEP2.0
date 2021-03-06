﻿using AccesoDatos;
using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicios
{
    /// <summary>
    /// Adrián Serrano
    /// 17/may/2019
    /// Clase para administrar los servicios de Proyecto
    /// </summary>
    public class ProyectoServicios
    {
        ProyectoDatos proyectoDatos = new ProyectoDatos();

        public LinkedList<Proyectos> ObtenerPorPeriodo(int anoPeriodo)
        {
            return proyectoDatos.ObtenerPorPeriodo(anoPeriodo);
        }

        public Proyectos ObtenerPorId(int idProyecto)
        {
            return proyectoDatos.ObtenerPorId(idProyecto);
        }

        public int Insertar(Proyectos proyecto)
        {
            return proyectoDatos.Insertar(proyecto);
        }

        public int Guardar(LinkedList<int> proyectosId, int anoPeriodo)
        {
            return proyectoDatos.Guardar(proyectosId, anoPeriodo);
        }

        public void EliminarProyecto(int idProyecto)
        {
            proyectoDatos.EliminarProyecto(idProyecto);
        }

        public void ActualizarProyecto(Proyectos proyecto)
        {
            proyectoDatos.ActualizarProyecto(proyecto);
        }

        /// <summary>
        /// Leonardo Carrion
        /// 27/sep/2019
        /// Efecto: devuelve los proyectos que se encuentran en el periodo consultado
        /// Requiere: periodo
        /// Modifica: -
        /// Devuelve: lista de proyectos
        /// </summary>
        /// <param name="periodo"></param>
        /// <returns></returns>
        public List<Proyectos> getProyectosPorPeriodo(Periodo periodo)
        {
            return proyectoDatos.getProyectosPorPeriodo(periodo);
        }
    }
}
