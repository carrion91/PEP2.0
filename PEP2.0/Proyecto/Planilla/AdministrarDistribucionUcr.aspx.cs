﻿using Entidades;
using PEP;
using Servicios;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Proyecto.Planilla
{
    public partial class AdministrarDistribucionUcr : System.Web.UI.Page
    {

        #region variables globales
        private FuncionarioServicios funcionarioServicios = new FuncionarioServicios();
        private ProyectoServicios proyectoServicios = new ProyectoServicios();
        private PlanillaServicios planillaServicios = new PlanillaServicios();
        private UnidadServicios unidadServicios = new UnidadServicios();
        private JornadaServicios jornadaServicios = new JornadaServicios();
        private JornadaUnidadFuncionarioServicios jornadaUnidadFuncionarioServicios = new JornadaUnidadFuncionarioServicios();
        private ProyeccionServicios proyeccionServicios = new ProyeccionServicios();
        private Proyeccion_CargaSocialServicios proyeccion_CargaSocialServicios = new Proyeccion_CargaSocialServicios();
        private EstadoPresupuestoServicios estadoPresupuestoServicios = new EstadoPresupuestoServicios();
        private PresupuestoIngresoServicios presupuestoIngresoServicios = new PresupuestoIngresoServicios();
        private PresupuestoEgresosServicios presupuestoEgresosServicios = new PresupuestoEgresosServicios();
        private PresupuestoEgreso_PartidaServicios presupuestoEgreso_PartidaServicios = new PresupuestoEgreso_PartidaServicios();
        private PartidaServicios partidaServicios = new PartidaServicios();
        #endregion

        #region variables globales paginacion Funcionarios
        readonly PagedDataSource pgsource = new PagedDataSource();
        int primerIndex, ultimoIndex;
        private int elmentosMostrar = 10;
        private int paginaActual
        {
            get
            {
                if (ViewState["paginaActual"] == null)
                {
                    return 0;
                }
                return ((int)ViewState["paginaActual"]);
            }
            set
            {
                ViewState["paginaActual"] = value;
            }
        }
        #endregion

        #region variables globales paginacion unidades
        readonly PagedDataSource pgsourceUnidad = new PagedDataSource();
        int primerIndexUnidad, ultimoIndexUnidad;
        private int paginaActualUnidad
        {
            get
            {
                if (ViewState["paginaActualUnidad"] == null)
                {
                    return 0;
                }
                return ((int)ViewState["paginaActualUnidad"]);
            }
            set
            {
                ViewState["paginaActualUnidad"] = value;
            }
        }
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
                //llenar drop down list
                List<Entidades.Planilla> periodos = planillaServicios.getPlanillas();
                ddlPeriodo.DataValueField = "idPlanilla";
                ddlPeriodo.DataTextField = "periodo";
                ddlPeriodo.DataSource = from a in periodos select new { a.idPlanilla, periodo = a.periodo.anoPeriodo };
                ddlPeriodo.SelectedValue = periodos.First().idPlanilla.ToString();
                ddlPeriodo.DataBind();
                LinkedList<Proyectos> proyectos = proyectoServicios.ObtenerPorPeriodo(periodos.First().periodo.anoPeriodo);
                List<Proyectos> listaProyectos = (List<Proyectos>)proyectos.Where(proy => proy.esUCR == true).ToList();
                ddlProyecto.DataSource = listaProyectos;
                ddlProyecto.DataTextField = "nombreProyecto";
                ddlProyecto.DataValueField = "idProyecto";
                ddlProyecto.SelectedValue = proyectos.First.Value.idProyecto.ToString();
                ddlProyecto.DataBind();
                Session["listaUnidades"] = unidadServicios.ObtenerPorProyecto(Convert.ToInt32(ddlProyecto.SelectedValue));
                Session["listaUnidadesConJornadaAsignada"] = new List<JornadaUnidadFuncionario>();
                mostrarTablaUnidades();
                //List<Funcionario> listaFuncionarios = funcionarioServicios.getFuncionarios(periodos.First().idPlanilla);
                List<Funcionario> listaFuncionarios = funcionarioServicios.getFuncionariosPorPlanillaYDistribuccion(periodos.First().idPlanilla);
                Session["listaFuncionarios"] = listaFuncionarios;
                Session["listaFuncionariosFiltrada"] = listaFuncionarios;
                mostrarDatosTabla();
            }
        }
        #endregion

        #region logica

        /// <summary>
        /// Leonardo Carrion
        /// 14/jun/2019
        /// Efecto: carga los datos filtrados en la tabla y realiza la paginacion correspondiente
        /// Requiere: -
        /// Modifica: los datos mostrados en pantalla
        /// Devuelve: -
        /// </summary>
        public void mostrarDatosTabla()
        {
            List<Funcionario> listaFuncionarios = (List<Funcionario>)Session["listaFuncionarios"];

            String nombre = "";

            if (!String.IsNullOrEmpty(txtBuscarNombre.Text))
            {
                nombre = txtBuscarNombre.Text;
            }

            List<Funcionario> listaFuncionarioFiltrada = (List<Funcionario>)listaFuncionarios.Where(funcionario => funcionario.nombreFuncionario.ToString().Contains(nombre)).ToList();

            Session["listaFuncionariosFiltrada"] = listaFuncionarioFiltrada;

            var dt = listaFuncionarioFiltrada;
            pgsource.DataSource = dt;
            pgsource.AllowPaging = true;
            //numero de items que se muestran en el Repeater
            pgsource.PageSize = elmentosMostrar;
            pgsource.CurrentPageIndex = paginaActual;
            //mantiene el total de paginas en View State
            ViewState["TotalPaginas"] = pgsource.PageCount;
            //Ejemplo: "Página 1 al 10"
            lblpagina.Text = "Página " + (paginaActual + 1) + " de " + pgsource.PageCount + " (" + dt.Count + " - elementos)";
            //Habilitar los botones primero, último, anterior y siguiente
            lbAnterior.Enabled = !pgsource.IsFirstPage;
            lbSiguiente.Enabled = !pgsource.IsLastPage;
            lbPrimero.Enabled = !pgsource.IsFirstPage;
            lbUltimo.Enabled = !pgsource.IsLastPage;

            rpFuncionarios.DataSource = pgsource;
            rpFuncionarios.DataBind();

            //metodo que realiza la paginacion
            Paginacion();
        }

        /// <summary>
        /// Leonardo Carrion
        /// 14/jun/2019
        /// Efecto: realiza la paginacion
        /// Requiere: -
        /// Modifica: paginacion mostrada en pantalla
        /// Devuelve: -
        /// </summary>
        private void Paginacion()
        {
            var dt = new DataTable();
            dt.Columns.Add("IndexPagina"); //Inicia en 0
            dt.Columns.Add("PaginaText"); //Inicia en 1

            primerIndex = paginaActual - 2;
            if (paginaActual > 2)
                ultimoIndex = paginaActual + 2;
            else
                ultimoIndex = 4;

            //se revisa que la ultima pagina sea menor que el total de paginas a mostrar, sino se resta para que muestre bien la paginacion
            if (ultimoIndex > Convert.ToInt32(ViewState["TotalPaginas"]))
            {
                ultimoIndex = Convert.ToInt32(ViewState["TotalPaginas"]);
                primerIndex = ultimoIndex - 4;
            }

            if (primerIndex < 0)
                primerIndex = 0;

            //se crea el numero de paginas basado en la primera y ultima pagina
            for (var i = primerIndex; i < ultimoIndex; i++)
            {
                var dr = dt.NewRow();
                dr[0] = i;
                dr[1] = i + 1;
                dt.Rows.Add(dr);
            }

            rptPaginacion.DataSource = dt;
            rptPaginacion.DataBind();
        }

        /// <summary>
        /// Jean Carlos Monge Mendez
        /// 18/10/2019
        /// Efecto : Llena el progress bar del modal Distribuir jornada
        /// Requiere : -
        /// Modifica : Progress bar del modal Distribuir Jornada
        /// Devuelve : -
        /// </summary>
        private void llenarProgressBar()
        {
            int idProyecto = Convert.ToInt32(ddlProyecto.SelectedValue);
            int idFuncionario = Convert.ToInt32(Session["idFuncionarioSeleccionado"]);
            System.Web.Script.Serialization.JavaScriptSerializer oSerializer =
            new System.Web.Script.Serialization.JavaScriptSerializer();
            List<JornadaUnidadFuncionario> unidadesFuncionario = jornadaUnidadFuncionarioServicios.getJornadaUnidadFuncionario(idFuncionario, idProyecto);
            Session["listaUnidadesConJornadaAsignada"] = unidadesFuncionario;
            string sJSON = oSerializer.Serialize(unidadesFuncionario);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "activar", "agregarDistribucion('" + sJSON + "');", true);
        }

        /// <summary>
        /// Jean Carlos Monge Mendez
        /// 18/10/2019
        /// Efecto : elimina una jornada laboral de un funcionario en una unidad
        /// Requiere : id de la unidad que se desea eliminar
        /// Modifica : Lista de jornadas de un funcionario
        /// Devuelve : true si se eliminó correctamente, false si no existe en la lista de jornadas asignadas
        /// </summary>
        /// <param name="idUnidad"></param>
        /// <returns></returns>
        private bool eliminarJornadaUnidadFuncionario(int idUnidad)
        {
            bool result = false;
            int idFuncionario = Convert.ToInt32(Session["idFuncionarioSeleccionado"]);
            List<JornadaUnidadFuncionario> unidadesFuncionario = (List<JornadaUnidadFuncionario>)Session["listaUnidadesConJornadaAsignada"];
            if (unidadesFuncionario.Any(x => x.idUnidad == idUnidad))
            {
                unidadesFuncionario.RemoveAll(x => x.idUnidad == idUnidad);
                JornadaUnidadFuncionario jornadaEliminar = new JornadaUnidadFuncionario();
                jornadaEliminar.idFuncionario = idFuncionario;
                jornadaEliminar.idUnidad = idUnidad;
                jornadaUnidadFuncionarioServicios.eliminarJornadaUnidadFuncionario(jornadaEliminar);
                result = true;
            }
            return result;
        }

        #endregion

        #region eventos

        #region paginacion tabla funcionarios
        /// <summary>
        /// Leonardo Carrion
        /// 14/jun/2019
        /// Efecto: se devuelve a la primera pagina y muestra los datos de la misma
        /// Requiere: dar clic al boton de "Primer pagina"
        /// Modifica: elementos mostrados en la tabla de contactos
        /// Devuelve: -
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lbPrimero_Click(object sender, EventArgs e)
        {
            paginaActual = 0;
            mostrarDatosTabla();
        }

        /// <summary>
        /// Leonardo Carrion
        /// 14/jun/2019
        /// Efecto: se devuelve a la ultima pagina y muestra los datos de la misma
        /// Requiere: dar clic al boton de "Ultima pagina"
        /// Modifica: elementos mostrados en la tabla de contactos
        /// Devuelve: -
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lbUltimo_Click(object sender, EventArgs e)
        {
            paginaActual = (Convert.ToInt32(ViewState["TotalPaginas"]) - 1);
            mostrarDatosTabla();
        }

        /// <summary>
        /// Leonardo Carrion
        /// 14/jun/2019
        /// Efecto: se devuelve a la pagina anterior y muestra los datos de la misma
        /// Requiere: dar clic al boton de "Anterior pagina"
        /// Modifica: elementos mostrados en la tabla de contactos
        /// Devuelve: -
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lbAnterior_Click(object sender, EventArgs e)
        {
            paginaActual -= 1;
            mostrarDatosTabla();
        }

        /// <summary>
        /// Leonardo Carrion
        /// 14/jun/2019
        /// Efecto: se devuelve a la pagina siguiente y muestra los datos de la misma
        /// Requiere: dar clic al boton de "Siguiente pagina"
        /// Modifica: elementos mostrados en la tabla de contactos
        /// Devuelve: -
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lbSiguiente_Click(object sender, EventArgs e)
        {
            paginaActual += 1;
            mostrarDatosTabla();
        }

        /// <summary>
        /// Leonardo Carrion
        /// 14/jun/2019
        /// Efecto: actualiza la la pagina actual y muestra los datos de la misma
        /// Requiere: -
        /// Modifica: elementos de la tabla
        /// Devuelve: -
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void rptPaginacion_ItemCommand(object source, DataListCommandEventArgs e)
        {
            if (!e.CommandName.Equals("nuevaPagina")) return;
            paginaActual = Convert.ToInt32(e.CommandArgument.ToString());
            mostrarDatosTabla();
        }

        /// <summary>
        /// Leonardo Carrion
        /// 14/jun/2019
        /// Efecto: marca el boton de la pagina seleccionada
        /// Requiere: dar clic al boton de paginacion
        /// Modifica: color del boton seleccionado
        /// Devuelve: -
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rptPaginacion_ItemDataBound(object sender, DataListItemEventArgs e)
        {
            var lnkPagina = (LinkButton)e.Item.FindControl("lbPaginacion");
            if (lnkPagina.CommandArgument != paginaActual.ToString()) return;
            lnkPagina.Enabled = false;
            lnkPagina.BackColor = Color.FromName("#005da4");
            lnkPagina.ForeColor = Color.FromName("#FFFFFF");
        }
        #endregion

        #region paginacion tabla unidades

        /// <summary>
        /// Jean Carlos Monge Mendez
        /// 07/10/2019
        /// Efecto : Llena la tabla con las unidades de un proyecto
        /// Requiere : -
        /// Modifica : Tabla de unidades
        /// Devuelve : -
        /// </summary>
        public void mostrarTablaUnidades()
        {
            List<Unidad> listaUnidades = unidadServicios.ObtenerPorProyecto(Convert.ToInt32(ddlProyecto.SelectedValue));
            /*FILTRO*/

            var dt = listaUnidades;
            pgsourceUnidad.DataSource = dt;
            pgsourceUnidad.AllowPaging = true;
            //numero de items que se muestran en el Repeater
            pgsourceUnidad.PageSize = elmentosMostrar;
            pgsourceUnidad.CurrentPageIndex = paginaActualUnidad;
            //mantiene el total de paginas en View State
            ViewState["TotalPaginasUnidad"] = pgsourceUnidad.PageCount;
            //Ejemplo: "Página 1 al 10"
            lblpaginaUnidad.Text = "Página " + (paginaActualUnidad + 1) + " de " + pgsourceUnidad.PageCount + " (" + dt.Count + " - elementos)";
            //Habilitar los botones primero, último, anterior y siguiente
            lbAnteriorUnidad.Enabled = !pgsourceUnidad.IsFirstPage;
            lbSiguienteUnidad.Enabled = !pgsourceUnidad.IsLastPage;
            lbPrimeroUnidad.Enabled = !pgsourceUnidad.IsFirstPage;
            lbUltimoUnidad.Enabled = !pgsourceUnidad.IsLastPage;

            rpUnidProyecto.DataSource = pgsourceUnidad;
            rpUnidProyecto.DataBind();

            //metodo que realiza la paginacion
            PaginacionUnidad();
        }

        /// <summary>
        /// Jean Carlos Monge Mendez
        /// 07/10/2019
        /// Efecto : Realiza la paginacion en la tabla de unidades
        /// Requiere : -
        /// Modifica : Tabla de unidades
        /// Devuelve : -
        /// </summary>
        private void PaginacionUnidad()
        {
            var dt = new DataTable();
            dt.Columns.Add("IndexPagina"); //Inicia en 0
            dt.Columns.Add("PaginaText"); //Inicia en 1

            primerIndexUnidad = paginaActualUnidad - 2;
            if (paginaActualUnidad > 2)
                ultimoIndexUnidad = paginaActualUnidad + 2;
            else
                ultimoIndexUnidad = 4;

            //se revisa que la ultima pagina sea menor que el total de paginas a mostrar, sino se resta para que muestre bien la paginacion
            if (ultimoIndexUnidad > Convert.ToInt32(ViewState["TotalPaginasUnidad"]))
            {
                ultimoIndexUnidad = Convert.ToInt32(ViewState["TotalPaginasUnidad"]);
                primerIndexUnidad = ultimoIndexUnidad - 4;
            }

            if (primerIndexUnidad < 0)
                primerIndexUnidad = 0;

            //se crea el numero de paginas basado en la primera y ultima pagina
            for (var i = primerIndexUnidad; i < ultimoIndexUnidad; i++)
            {
                var dr = dt.NewRow();
                dr[0] = i;
                dr[1] = i + 1;
                dt.Rows.Add(dr);
            }

            rptPaginacionUnidad.DataSource = dt;
            rptPaginacionUnidad.DataBind();
        }

        /// <summary>
        /// Leonardo Carrion
        /// 16/jul/2019
        /// Efecto: se devuelve a la primera pagina y muestra los datos de la misma
        /// Requiere: dar clic al boton de "Primer pagina"
        /// Modifica: elementos mostrados en la tabla de notas
        /// Devuelve: -
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lbPrimeroUnidad_Click(object sender, EventArgs e)
        {
            paginaActualUnidad = 0;
            mostrarTablaUnidades();
        }

        /// <summary>
        /// Leonardo Carrion
        /// 16/jul/2019
        /// Efecto: se devuelve a la pagina anterior y muestra los datos de la misma
        /// Requiere: dar clic al boton de "Anterior pagina"
        /// Modifica: elementos mostrados en la tabla de notas
        /// Devuelve: -
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lbAnteriorUnidad_Click(object sender, EventArgs e)
        {
            paginaActualUnidad -= 1;
            mostrarTablaUnidades();
        }

        /// <summary>
        /// Leonardo Carrion
        /// 16/jul/2019
        /// Efecto: actualiza la la pagina actual y muestra los datos de la misma
        /// Requiere: -
        /// Modifica: elementos de la tabla
        /// Devuelve: -
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void rptPaginacionUnidad_ItemCommand(object source, DataListCommandEventArgs e)
        {
            if (!e.CommandName.Equals("nuevaPagina")) return;
            paginaActualUnidad = Convert.ToInt32(e.CommandArgument.ToString());
            mostrarTablaUnidades();
        }

        /// <summary>
        /// Leonardo Carrion
        /// 16/jul/2019
        /// Efecto: marca el boton de la pagina seleccionada
        /// Requiere: dar clic al boton de paginacion
        /// Modifica: color del boton seleccionado
        /// Devuelve: -
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rptPaginacionUnidad_ItemDataBound(object sender, DataListItemEventArgs e)
        {
            var lnkPagina = (LinkButton)e.Item.FindControl("lbPaginacionUnidad");
            if (lnkPagina.CommandArgument != paginaActualUnidad.ToString()) return;
            lnkPagina.Enabled = false;
            lnkPagina.BackColor = Color.FromName("#00Unidadda4");
            lnkPagina.ForeColor = Color.FromName("#000000");
        }

        /// <summary>
        /// Leonardo Carrion
        /// 14/jun/2019
        /// Efecto: se devuelve a la pagina siguiente y muestra los datos de la misma
        /// Requiere: dar clic al boton de "Siguiente pagina"
        /// Modifica: elementos mostrados en la tabla 
        /// Devuelve: -
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lbSiguienteUnidad_Click(object sender, EventArgs e)
        {
            paginaActualUnidad += 1;
            mostrarTablaUnidades();
        }

        /// <summary>
        /// Leonardo Carrion
        /// 16/jul/2019
        /// Efecto: se devuelve a la ultima pagina y muestra los datos de la misma
        /// Requiere: dar clic al boton de "Ultima pagina"
        /// Modifica: elementos mostrados en la tabla de notas
        /// Devuelve: -lbPrimero
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lbUltimoUnidad_Click(object sender, EventArgs e)
        {
            paginaActualUnidad = (Convert.ToInt32(ViewState["TotalPaginasUnidad"]) - 1);
            mostrarTablaUnidades();
        }
        #endregion FIN paginacion tabla unidades

        /// <summary>
        /// Jean Carlos Monge Mendez
        /// 20/09/2019
        /// Efecto : Muestra los datos del funcionario seleccionado
        /// Requiere : Clickear el boton "Seleccionar"
        /// Modifica : -
        /// Devuelve : -
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSelccionarFuncionario_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32((((LinkButton)(sender)).CommandArgument).ToString());
            Funcionario funcionarioVer = null;
            List<Funcionario> funcionarios = (List<Funcionario>)Session["listaFuncionariosFiltrada"];
            foreach (Funcionario funcionario in funcionarios)
            {
                if (funcionario.idFuncionario == id)
                {
                    funcionarioVer = funcionario;
                    break;
                }
            }
            lblPeriodo.Text = ddlPeriodo.SelectedValue;
            lblProyecto.Text = ddlProyecto.SelectedItem.Text;
            lblJornada.Text = funcionarioVer.JornadaLaboral.descJornada + " , " + funcionarioVer.JornadaLaboral.porcentajeJornada + "%";
            lblFuncionario.Text = funcionarioVer.nombreFuncionario;
            Session["idFuncionarioSeleccionado"] = funcionarioVer.idFuncionario;
            mostrarTablaUnidades();
            llenarProgressBar();
        }

        /// <summary>
        /// Leonardo Carrion
        /// 16/jul/2019
        /// Efecto: devuelve a la pantalla de administrar planillas
        /// Requiere: dar clic al boton de "regresar"
        /// Modifica: -
        /// Devuelve: -
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnRegresar_Click(object sender, EventArgs e)
        {
            String url = Page.ResolveUrl("~/Default.aspx");
            Response.Redirect(url);
        }

        /// <summary>
        /// Jean Carlos Monge Mendez
        /// 18/10/2019
        /// Efecto : Al cerrar el modal de asgnar jornada, recupera el estado del modal Distribuir jornada
        /// Requiere : Clickear el botón "Cerrar" del modal Asignar jornada
        /// Modifica : -
        /// Devuelve : -
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnCerrarModalAsignarJornada_Click(object sender, EventArgs e)
        {
            llenarProgressBar();
        }

        /// <summary>
        /// Jean Carlos Monge Mendez
        /// 07/10/2019
        /// Efecto : Completa el formulario para asinar una jornada laboral a una unidad
        /// Requiere : Seleccionar una unidad
        /// Modifica : Formulario de asignar jornada 
        /// Devuelve : -
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSelccionarUnidad_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32((((LinkButton)(sender)).CommandArgument).ToString());
            Unidad unidadSeleccionada = null;
            List<Unidad> unidades = unidadServicios.ObtenerPorProyecto(Convert.ToInt32(ddlProyecto.SelectedValue));
            foreach (Unidad unidad in unidades)
            {
                if (unidad.idUnidad == id)
                {
                    unidadSeleccionada = unidad;
                    break;
                }
            }
            IdUnidadSeleccionada.Value = unidadSeleccionada.idUnidad.ToString();
            lblUnidad.Text = unidadSeleccionada.nombreUnidad;
            List<Jornada> jornadas = jornadaServicios.getJornadasActivas();
            ddlAsignarJornada.DataSource = jornadas;
            ddlAsignarJornada.DataTextField = "porcentajeJornada";
            ddlAsignarJornada.DataValueField = "idJornada";
            ddlAsignarJornada.DataBind();
            mostrarTablaUnidades();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "activar", "activarModalAsignarJornada();", true);
        }

        /// <summary>
        /// Jean Carlos Monge Mendez
        /// 07/10/2019
        /// Efecto : Asigna una jornada laboral a la unidad seleccionada 
        /// Requiere : Seleccionar la jornada laboral y clickear el boton "Asignar" del formulario
        /// Modifica : Jornada laboral del funcionario
        /// Devuelve : -
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnAsignarJornada_Click(object sender, EventArgs e)
        {
            int idUnidad = Convert.ToInt32(IdUnidadSeleccionada.Value);
            string unidad = lblUnidad.Text;
            double porcentaje = Convert.ToDouble(ddlAsignarJornada.SelectedItem.Text);
            JornadaUnidadFuncionario unidadAsignada = new JornadaUnidadFuncionario();
            unidadAsignada.idUnidad = idUnidad;
            unidadAsignada.idJornada = Convert.ToInt32(ddlAsignarJornada.SelectedValue);
            unidadAsignada.jornadaAsignada = porcentaje;
            unidadAsignada.idFuncionario = Convert.ToInt32(Session["idFuncionarioSeleccionado"]);
            unidadAsignada.descUnidad = unidad;
            eliminarJornadaUnidadFuncionario(idUnidad);
            List<JornadaUnidadFuncionario> unidadesFuncionario = (List<JornadaUnidadFuncionario>)Session["listaUnidadesConJornadaAsignada"];
            double tiempoAsignado = 0;
            foreach (JornadaUnidadFuncionario unidadFuncionario in unidadesFuncionario)
            {
                tiempoAsignado += unidadFuncionario.jornadaAsignada;
            }
            if ((tiempoAsignado + unidadAsignada.jornadaAsignada) <= 100)
            {
                unidadesFuncionario.Add(unidadAsignada);
                jornadaUnidadFuncionarioServicios.insertarJornadaUnidadFuncionario(unidadAsignada);
                Toastr("success", "distribución en " + unidadAsignada.descUnidad + " agregada con éxito!");

                List<Funcionario> listaFuncionarios = funcionarioServicios.getFuncionariosPorPlanillaYDistribuccion(Convert.ToInt32(ddlPeriodo.SelectedValue));
                Session["listaFuncionarios"] = listaFuncionarios;
                Session["listaFuncionariosFiltrada"] = listaFuncionarios;
                mostrarDatosTabla();
            }
            else
            {
                Toastr("error", "Se sobrepasa el tiempo disponible");
            }
            llenarProgressBar();
        }

        /// <summary>
        /// Jean Carlos Monge Mendez
        /// 11/10/2019
        /// Efecto : Remueve una jornada de unidad del funcionario
        /// Requiere : Clickear el boton "Eliminar jornada" del formulario
        /// Modifica : Lista de jornadas de un funcionario
        /// Devuelve : -
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnEliminarUnidad_Click(object sender, EventArgs e)
        {
            int idUnidad = Convert.ToInt32((((LinkButton)(sender)).CommandArgument).ToString());
            if (eliminarJornadaUnidadFuncionario(idUnidad))
            {
                Toastr("success", "Se eliminó la jornada con éxito!");
            }
            else
            {
                Toastr("error", "La jornada no se ha asignado");
            }
            llenarProgressBar();
        }

        /// <summary>
        /// Jean Carlos Monge Mendez
        /// 27/09/2019
        /// Efecto : Actualiza la lista de proyectos de un periodo
        /// Requiere : Cambiar el periodo
        /// Modifica : Lista de proyectos 
        /// Devuelve : -
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlPeriodo_SelectedIndexChanged(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(ddlPeriodo.SelectedItem.Text);
            LinkedList<Proyectos> proyectos = proyectoServicios.ObtenerPorPeriodo(id);
            List<Proyectos> listaProyectos = (List<Proyectos>)proyectos.Where(proy => proy.esUCR == true).ToList();
            ddlProyecto.DataSource = listaProyectos;
            ddlProyecto.DataBind();
            List<Funcionario> listaFuncionarios = funcionarioServicios.getFuncionariosPorPlanillaYDistribuccion(Convert.ToInt32(ddlPeriodo.SelectedValue));
            Session["listaFuncionarios"] = listaFuncionarios;
            Session["listaFuncionariosFiltrada"] = listaFuncionarios;
            mostrarDatosTabla();
        }

        /// <summary>
        /// Leonardo Carrion
        /// 12/jun/2019
        /// Efecto: filtra la tabla segun los datos ingresados en los filtros
        /// Requiere: dar clic en el boton de flitrar e ingresar datos en los filtros
        /// Modifica: datos de la tabla
        /// Devuelve: -
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnFiltrar_Click(object sender, EventArgs e)
        {
            paginaActual = 0;
            mostrarDatosTabla();
        }

        /// <summary>
        /// Leonardo Carrion
        /// 08/nov/2019
        /// Efecto: ingresa en el presupuesto de egresos los montos en las partidas y unidades correspondientes
        /// Requiere: dar clic en el boton de "Ingresar datos al presupuesto de egresos" y haber distribuido todos los funcionarios
        /// Modifica: presupuesto de egresos
        /// Devuelve: -
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnIngresarPresupuestoEgresos_Click(object sender, EventArgs e)
        {
            List<Funcionario> listaFuncionarios = (List<Funcionario>)Session["listaFuncionarios"];
            List<Funcionario> listaFuncionariosTemp = listaFuncionarios.Where(funcionario => funcionario.porcentajeAsignado < 100).ToList();

            if (listaFuncionariosTemp.Count > 0)
            {
                Toastr("error", "Se deben de distribuir todos los funcionarios completamente");
            }
            else
            {
                int idProyecto = Convert.ToInt32(ddlProyecto.SelectedValue);
                int id = Convert.ToInt32(ddlPeriodo.SelectedItem.Text);
                Periodo periodo = new Periodo();
                periodo.anoPeriodo = id;

                Proyectos proyecto = new Proyectos();
                proyecto.idProyecto = idProyecto;

                //primero se comprueba que el monto de que se va a sumar a los egresos no sobrepase el monto de ingresos
                Double montoSumaEgresos = 0;
                foreach (Funcionario funcionario in listaFuncionarios)
                {
                    List<Proyeccion> listaProyeccion = proyeccionServicios.getProyeccionesPorPeriodoYFuncionario(periodo, funcionario);

                    montoSumaEgresos += listaProyeccion.Sum(proyeccion => proyeccion.montoCargasTotal);
                    montoSumaEgresos += listaProyeccion.Sum(proyeccion => proyeccion.montoSalario);
                }

                List<Unidad> listaUnidades = unidadServicios.ObtenerPorProyecto(idProyecto);

                List<PresupuestoEgresoPartida> listaPresupuestoEgresoPartidasEliminar = new List<PresupuestoEgresoPartida>();//lista de presupuestos partidas ya aprobados, hay q borrar si es el caso
                List<int> listaIdEgresos = new List<int>();

                foreach (Unidad unidad in listaUnidades)
                {
                    List<PresupuestoEgreso> listaPresupuestoEgresos = presupuestoEgresosServicios.getPresupuestosEgresosPorUnidad(unidad);
                    listaIdEgresos = (List<int>)listaPresupuestoEgresos.Select(pres => pres.idPresupuestoEgreso).ToList();
                    listaIdEgresos = (List<int>)listaIdEgresos.Distinct().ToList();
                    foreach (int idEgreso in listaIdEgresos)
                    {
                        PresupuestoEgreso presupuestoEgreso = new PresupuestoEgreso();
                        presupuestoEgreso.idPresupuestoEgreso = idEgreso;
                        listaPresupuestoEgresoPartidasEliminar.AddRange(presupuestoEgreso_PartidaServicios.getPresupuestoEgresoPartidasPorPresupEgresoYDesc(presupuestoEgreso, "Planilla (generado automáticamente)"));
                    }
                    montoSumaEgresos += listaPresupuestoEgresos.Sum(presupuesto => presupuesto.montoTotal);
                }

                Double montoIngresos = 0;
                List<Entidades.PresupuestoIngreso> listaPresupuestosIngresos = presupuestoIngresoServicios.getPresupuestosIngresosPorProyecto(proyecto);

                foreach (Entidades.PresupuestoIngreso presupuestoIngreso in listaPresupuestosIngresos)
                {
                    if (!presupuestoIngreso.estadoPresupIngreso.descEstado.Equals("Guardar"))
                    {
                        montoIngresos += presupuestoIngreso.monto;
                    }
                }

                montoSumaEgresos = montoSumaEgresos - (listaPresupuestoEgresoPartidasEliminar.Sum(pres => pres.monto));

                if (montoIngresos < montoSumaEgresos)
                {
                    Toastr("error", "El monto de ingresos es menor que el monto que se va a egregar a los egresos. Monto ingresos: " + montoIngresos + " Monto egresos: " + montoSumaEgresos);
                }
                else
                { //si se puede ingresar los montos

                    //se actualiza el monto de los egresos para eliminar la distribucion hecha antes
                    foreach (int idEgreso in listaIdEgresos)
                    {
                        List<PresupuestoEgresoPartida> presupuestoEgresoPartidasTemp = (List < PresupuestoEgresoPartida >)listaPresupuestoEgresoPartidasEliminar.Where(pres => pres.idPresupuestoEgreso==idEgreso).ToList();

                        PresupuestoEgreso presupuestoEgreso = new PresupuestoEgreso();
                        presupuestoEgreso.idPresupuestoEgreso = idEgreso;
                        presupuestoEgreso = presupuestoEgresosServicios.getPresupuestosEgresosPorId(presupuestoEgreso);
                        presupuestoEgreso.montoTotal = presupuestoEgreso.montoTotal - (presupuestoEgresoPartidasTemp.Sum(pres => pres.monto));
                        presupuestoEgresosServicios.actualizarMontoPresupuestoEgreso(presupuestoEgreso);
                    }

                    //se elimina el presupuesto de egresos partidas ya ingresados, para que no queden cosas sucias en la base de datos
                    foreach (PresupuestoEgresoPartida presupuestoEgresoPartidaEliminar in listaPresupuestoEgresoPartidasEliminar)
                    {
                        presupuestoEgreso_PartidaServicios.eliminarPresupuestoEgreso_Partida(presupuestoEgresoPartidaEliminar);
                    }

                    List<PresupuestoEgresoPartida> presupuestoEgresoPartidas = new List<PresupuestoEgresoPartida>();
                    EstadoPresupuesto estadoPresupuesto = new EstadoPresupuesto();
                    estadoPresupuesto = estadoPresupuestoServicios.getEstadoPresupuestoPorNombre("Aprobar");

                    foreach (Funcionario funcionario in listaFuncionarios)
                    {
                        List<JornadaUnidadFuncionario> unidadesFuncionario = jornadaUnidadFuncionarioServicios.getJornadaUnidadFuncionario(funcionario.idFuncionario, idProyecto);
                        List<Proyeccion> listaProyeccion = proyeccionServicios.getProyeccionesPorPeriodoYFuncionario(periodo, funcionario);

                        foreach (Proyeccion proyeccion in listaProyeccion)
                        {
                            List<Proyeccion_CargaSocial> listaProyeccion_CargaSociales = proyeccion_CargaSocialServicios.getProyeccionCargaSocialPorProyeccionPorProyeccion(proyeccion);


                            foreach (JornadaUnidadFuncionario jornadaUnidadFuncionario in unidadesFuncionario)
                            {

                                Unidad unidad = new Unidad();
                                unidad.idUnidad = jornadaUnidadFuncionario.idUnidad;
                                List<PresupuestoEgreso> listaPresupuestoEgresos = presupuestoEgresosServicios.getPresupuestosEgresosPorUnidad(unidad);
                                PresupuestoEgreso presupuestoEgreso = listaPresupuestoEgresos.First();

                                foreach (Proyeccion_CargaSocial proyeccion_CargaSocial in listaProyeccion_CargaSociales)
                                {
                                    Double monto = 0;
                                    monto = proyeccion_CargaSocial.monto;
                                    monto = (monto * (jornadaUnidadFuncionario.jornadaAsignada / 100));

                                    PresupuestoEgresoPartida presupuestoEgresoPartida = new PresupuestoEgresoPartida();
                                    presupuestoEgresoPartida.monto = monto;
                                    presupuestoEgresoPartida.partida = proyeccion_CargaSocial.cargaSocial.partida;

                                    presupuestoEgresoPartida.idPresupuestoEgreso = presupuestoEgreso.idPresupuestoEgreso;

                                    presupuestoEgresoPartidas.Add(presupuestoEgresoPartida);
                                }
                                //salario y concepto de pago
                                Double montoSalario = 0, montoConceptoPago = 0;
                                montoConceptoPago = funcionario.conceptoPagoLey;
                                montoConceptoPago = (montoConceptoPago * (jornadaUnidadFuncionario.jornadaAsignada / 100));
                                montoSalario = proyeccion.montoSalario - funcionario.conceptoPagoLey;
                                montoSalario = (montoSalario * (jornadaUnidadFuncionario.jornadaAsignada / 100));

                                PresupuestoEgresoPartida presupuestoEgresoPartidaSalario = new PresupuestoEgresoPartida();
                                presupuestoEgresoPartidaSalario.monto = montoSalario;
                                Partida partida = new Partida();
                                partida.numeroPartida = "0-01-03-01";
                                partida = partidaServicios.getPartidaPorNumeroYPeriodo(partida, periodo);
                                presupuestoEgresoPartidaSalario.partida = partida;
                                presupuestoEgresoPartidaSalario.idPresupuestoEgreso = presupuestoEgreso.idPresupuestoEgreso;

                                presupuestoEgresoPartidas.Add(presupuestoEgresoPartidaSalario);

                                PresupuestoEgresoPartida presupuestoEgresoPartidaConcepto = new PresupuestoEgresoPartida();
                                presupuestoEgresoPartidaConcepto.monto = montoConceptoPago;
                                Partida partidaConcepto = new Partida();
                                partidaConcepto.numeroPartida = "0-01-03-02";
                                partidaConcepto = partidaServicios.getPartidaPorNumeroYPeriodo(partidaConcepto, periodo);
                                presupuestoEgresoPartidaConcepto.partida = partidaConcepto;
                                presupuestoEgresoPartidaConcepto.idPresupuestoEgreso = presupuestoEgreso.idPresupuestoEgreso;

                                presupuestoEgresoPartidas.Add(presupuestoEgresoPartidaConcepto);
                            }
                        }
                    }

                    //se seleccionan los id's de los egresos
                    List<int> egresos = (List<int>)presupuestoEgresoPartidas.Select(pres => pres.idPresupuestoEgreso).ToList();
                    egresos = (List<int>)egresos.Distinct().ToList();
                    List<int> partidas = (List<int>)presupuestoEgresoPartidas.Select(pres => pres.partida.idPartida).ToList();
                    partidas = (List<int>)partidas.Distinct().ToList();

                    foreach (int idEgreso in egresos)
                    {
                        PresupuestoEgreso presupuestoEgreso = new PresupuestoEgreso();
                        presupuestoEgreso.idPresupuestoEgreso = idEgreso;
                        presupuestoEgreso = presupuestoEgresosServicios.getPresupuestosEgresosPorId(presupuestoEgreso);


                        foreach (int idPartida in partidas)
                        {
                            List<PresupuestoEgresoPartida> presupuestoEgresoPartidasTemp = (List<PresupuestoEgresoPartida>)presupuestoEgresoPartidas.Where(pres => pres.idPresupuestoEgreso == idEgreso && pres.partida.idPartida == idPartida).ToList();
                            Double montoTemp = 0;
                            montoTemp += presupuestoEgresoPartidasTemp.Sum(pres => pres.monto);

                            if (montoTemp > 0)
                            {
                                PresupuestoEgresoPartida presupuestoEgresoPartida = new PresupuestoEgresoPartida();
                                Partida partida = new Partida();

                                partida.idPartida = idPartida;
                                presupuestoEgresoPartida.idPresupuestoEgreso = idEgreso;
                                presupuestoEgresoPartida.partida = partida;
                                presupuestoEgresoPartida.monto = montoTemp;
                                presupuestoEgresoPartida.estadoPresupuesto = estadoPresupuesto;
                                presupuestoEgresoPartida.descripcion = "Planilla (generado automáticamente)";

                                presupuestoEgreso_PartidaServicios.insertarPresupuestoEgreso_Partida(presupuestoEgresoPartida);

                                presupuestoEgreso.montoTotal += montoTemp;
                            }
                        }

                        presupuestoEgresosServicios.actualizarMontoPresupuestoEgreso(presupuestoEgreso);

                    }

                    Toastr("success", "Se ingreso correctamente los datos en el presupuesto de egresos");

                }

            }
        }

        #endregion

        #region mensaje toast

        private void Toastr(string tipo, string mensaje)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "toastr." + tipo + "('" + mensaje + "');", true);
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            ViewState["CheckRefresh"] = Session["CheckRefresh"];
        }
        #endregion
    }
}