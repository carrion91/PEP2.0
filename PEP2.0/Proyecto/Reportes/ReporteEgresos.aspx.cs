﻿using Entidades;
using Microsoft.Reporting.WebForms;
using PEP;
using Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Proyecto.Reportes
{
    public partial class ReporteEgresos : System.Web.UI.Page
    {
        #region variables globales
        PeriodoServicios periodoServicios = new PeriodoServicios();
        ProyectoServicios proyectoServicios = new ProyectoServicios();
        Reporte_EgresosServicios reporte_EgresosServicios = new Reporte_EgresosServicios();
        #endregion
        #region page load
        protected void Page_Load(object sender, EventArgs e)
        {
            //controla los menus q se muestran y las pantallas que se muestras segun el rol que tiene el usuario
            //si no tiene permiso de ver la pagina se redirecciona a login
            int[] rolesPermitidos = { 2 };
            Utilidades.escogerMenu(Page, rolesPermitidos);

            if (!IsPostBack)
            {
                llenarDdlPeriodos();

                Periodo periodo = new Periodo();
                periodo.anoPeriodo = Convert.ToInt32(ddlPeriodos.SelectedValue);

                Proyectos proyecto = new Proyectos();
                proyecto.idProyecto = Convert.ToInt32(ddlProyectos.SelectedValue);


                Session["listaReporteEgresos"] = reporte_EgresosServicios.getReporteEgresos(periodo,proyecto);

                cargarDatosReporte();
            }
        }
        #endregion
        #region logica
        /// <summary>
        /// Leonardo Carrion
        /// 21/oct/2019
        /// Efecto: llean el DropDownList con los periodos que se encuentran en la base de datos
        /// Requiere: - 
        /// Modifica: DropDownList
        /// Devuelve: -
        /// </summary>
        public void llenarDdlPeriodos()
        {
            LinkedList<Periodo> periodos = new LinkedList<Periodo>();
            ddlPeriodos.Items.Clear();
            periodos = this.periodoServicios.ObtenerTodos();
            int anoHabilitado = 0;

            if (periodos.Count > 0)
            {
                foreach (Periodo periodo in periodos)
                {
                    string nombre;

                    if (periodo.habilitado)
                    {
                        nombre = periodo.anoPeriodo.ToString() + " (Actual)";
                        anoHabilitado = periodo.anoPeriodo;
                    }
                    else
                    {
                        nombre = periodo.anoPeriodo.ToString();
                    }

                    ListItem itemPeriodo = new ListItem(nombre, periodo.anoPeriodo.ToString());
                    ddlPeriodos.Items.Add(itemPeriodo);
                }

                if (anoHabilitado != 0)
                {
                    ddlPeriodos.Items.FindByValue(anoHabilitado.ToString()).Selected = true;
                }
            }

            CargarProyectos();
        }

        /// <summary>
        /// Leonardo Carrion
        /// 21/oct/2019
        /// Efecto: llean el DropDownList con los proyectos que se encuentran en la base de datos segun el periodo seleccionado
        /// Requiere: Periodo
        /// Modifica: DropDownList y datos del reporte
        /// Devuelve: -
        /// </summary>
        private void CargarProyectos()
        {
            ddlProyectos.Items.Clear();

            if (!ddlPeriodos.SelectedValue.Equals(""))
            {
                LinkedList<Proyectos> proyectos = new LinkedList<Proyectos>();
                proyectos = proyectoServicios.ObtenerPorPeriodo(Int32.Parse(ddlPeriodos.SelectedValue));

                if (proyectos.Count > 0)
                {
                    foreach (Proyectos proyectoTemp in proyectos)
                    {
                        ListItem itemLB = new ListItem(proyectoTemp.nombreProyecto, proyectoTemp.idProyecto.ToString());
                        ddlProyectos.Items.Add(itemLB);
                    }

                    Periodo periodo = new Periodo();
                    periodo.anoPeriodo = Convert.ToInt32(ddlPeriodos.SelectedValue);

                    Proyectos proyecto = new Proyectos();
                    proyecto.idProyecto = Convert.ToInt32(ddlProyectos.SelectedValue);


                    Session["listaReporteEgresos"] = reporte_EgresosServicios.getReporteEgresos(periodo, proyecto);

                    cargarDatosReporte();
                }
            }
        }


        /// <summary>
        /// Leonardo Carrion
        /// 21/oct/2019
        /// Efecto: cargar el reporte con los datos filtrados
        /// Requiere: -
        /// Modifica: datasource del reporte
        /// Devuelve: -
        /// </summary>
        public void cargarDatosReporte()
        {
            ReportViewer1.Reset();
            ReportViewer1.ProcessingMode = ProcessingMode.Local;
            ReportViewer1.LocalReport.ReportPath = Server.MapPath("ReporteEgresos.rdlc");
            ReportDataSource reportDataSource = null;
            List<Reporte_Egresos> listaReporteDuracion = (List<Reporte_Egresos>)Session["listaReporteEgresos"];

            reportDataSource = new ReportDataSource("DataSet1", listaReporteDuracion);
            if (reportDataSource.Equals(null) == false)
                ReportViewer1.LocalReport.DataSources.Add(reportDataSource);
            ReportViewer1.DataBind();
        }
        #endregion
        #region eventos
        /// <summary>
        /// Leonardo Carrion
        /// 21/oct/2019
        /// Efecto: cambia los datos de la tabla segun el periodo seleccionado
        /// Requiere: cambiar periodo
        /// Modifica: datos de la tabla
        /// Devuelve: -
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlPeriodo_SelectedIndexChanged(object sender, EventArgs e)
        {
            Periodo periodo = new Periodo();
            periodo.anoPeriodo = Convert.ToInt32(ddlPeriodos.SelectedValue);

            CargarProyectos();
        }

        /// <summary>
        /// Leonardo Carrion
        /// 21/oct/2019
        /// Efecto: cargar la tabla de plan estrategico y muestra los montos de las partidas
        /// Requiere: cambiar la unidad
        /// Modifica: plan estrategico y montos de las partidas
        /// Devuelve: -
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlProyecto_SelectedIndexChanged(object sender, EventArgs e)
        {
            Periodo periodo = new Periodo();
            periodo.anoPeriodo = Convert.ToInt32(ddlPeriodos.SelectedValue);

            Proyectos proyecto = new Proyectos();
            proyecto.idProyecto = Convert.ToInt32(ddlProyectos.SelectedValue);


            Session["listaReporteEgresos"] = reporte_EgresosServicios.getReporteEgresos(periodo, proyecto);

            cargarDatosReporte();
        }
        #endregion
    }
}