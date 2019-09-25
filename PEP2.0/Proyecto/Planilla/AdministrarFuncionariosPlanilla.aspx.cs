﻿using PEP;
using Entidades;
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
    public partial class AdministrarFuncionariosPlanilla : System.Web.UI.Page
    {
        #region variables globales 
        private FuncionarioServicios funcionarioServicios = new FuncionarioServicios();
        private EscalaSalarialServicios escalaSalarialServicios = new EscalaSalarialServicios();
        private static Funcionario funcionarioSeleccionado = new Funcionario();
        private static EscalaSalarial escalaSeleccionada = new EscalaSalarial();
        #endregion

        #region variables globales paginacion
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

        #region page load
        protected void Page_Load(object sender, EventArgs e)
        {
            //controla los menus q se muestran y las pantallas que se muestras segun el rol que tiene el usuario
            //si no tiene permiso de ver la pagina se redirecciona a login
            int[] rolesPermitidos = { 2 };
            Utilidades.escogerMenu(Page, rolesPermitidos);
            if (!IsPostBack)
            {
                Entidades.Planilla planilla = (Entidades.Planilla)Session["planillaSeleccionada"];
                lblPeriodo.Text = planilla.periodo.anoPeriodo.ToString();
                lblAnualidad1.Text = planilla.anualidad1 + " %";
                lblAnualidad2.Text = planilla.anualidad2 + " %";

                List<Funcionario> listaFuncionarios = funcionarioServicios.getFuncionarios();

                Session["listaFuncionarios"] = listaFuncionarios;
                Session["listaFuncionariosFiltrada"] = listaFuncionarios;

                mostrarDatosTabla();

                List<EscalaSalarial> listaEscalas = escalaSalarialServicios.getEscalasSalarialesPorPeriodo(planilla.periodo);
                Session["listaEscalas"] = listaEscalas;
                ddlEscalaSalarial.DataSource = listaEscalas;
                ddlEscalaSalarial.DataTextField = "descEscalaSalarial";
                ddlEscalaSalarial.DataValueField = "idEscalaSalarial";
                ddlEscalaSalarial.DataBind();
            }
        }
        #endregion

        #region logica
        /// <summary>
        /// Leonardo Carrion
        /// 17/jul/2019
        /// Efecto: calcula el monto de la anualidad
        /// Requiere: porcentaje de anualidad, total salario base, monto de escalafones
        /// Modifica: -
        /// Devuelve: monto de anualidad
        /// </summary>
        /// <returns></returns>
        /// <param name="montoEscalafones"></param>
        /// <param name="porcentajeAnualidad"></param>
        /// <param name="sumaSalarioBase"></param>
        private Double calcularMontoAnualidad(double sumaSalarioBase, double montoEscalafones, double porcentajeAnualidad)
        {
            double montoAnualiadad = (sumaSalarioBase + montoEscalafones) * (porcentajeAnualidad / 100);
            return montoAnualiadad;
        }

        /// <summary>
        /// Jean Carlos Monge Mendez
        /// 04/09/2019
        /// Efecto: Calcula el salario de contratacion
        /// Requiere: total salario base, monto escalafones, monto anualidad
        /// Modifica : -
        /// Devuelve : Salario de contratacion
        /// </summary>
        /// <returns></returns>
        /// <param name="sumaSalarioBase"></param>
        /// <param name="montoEscalafones"></param>
        /// <param name="montoAnualidad"></param>
        private Double calcularSalarioContratacion(double sumaSalarioBase, double montoEscalafones, double montoAnualidad)
        {
            double salarioContratacion = sumaSalarioBase + montoEscalafones + montoAnualidad;
            return salarioContratacion;
        }

        /// <summary>
        /// Jean Carlos Monge Mendez
        /// 06/29/2019
        /// Efecto : Calcula el salario total de un funcionario
        /// Requiere : Salario base, suma al salario base
        /// Modifica : -
        /// Devuelve : Salario total
        /// </summary>
        /// <param name="salarioBase"></param>
        /// <param name="sumaSalarioBase"></param>
        /// <returns></returns>
        private Double calcularTotalSalarioBase(Double salarioBase, Double sumaSalarioBase)
        {
            Double total = 0;
            if (sumaSalarioBase == 0)
            {
                total = salarioBase;
            }
            else
            {
                total = salarioBase * sumaSalarioBase;
            }
            return total;
        }

        /// <summary>
        /// Leonardo Carrion
        /// 17/jul/2019
        /// Efecto: calcula el monto de escalafon
        /// Requiere: salario base, numero de escalafones
        /// Modifica: -
        /// Devuelve: monto de escalafones
        /// </summary>
        /// <param name="numeroEscalafones"></param>
        /// <param name="totalSalarioBase"></param>
        /// <returns></returns>
        private Double calcularMontoEscalafon(double SalarioBase, int numeroEscalafones)
        {
            double montoEscalafon = (SalarioBase * numeroEscalafones) * (escalaSeleccionada.porentajeEscalafones / 100);
            return montoEscalafon;
        }

        /// <summary>
        /// Jean Carlos Monge Mendez
        /// 04/09/2019
        /// Efecto : Calcula el salario mensual por semestre 
        /// Requiere : Salario contratacion, pago ley 8114
        /// Modifica : -
        /// Devuelve : Salario mensual del semestre
        /// </summary>
        /// <returns></returns>
        /// <param name="salarioContratacion"></param>
        /// <param name="pagoLey8114"></param>
        private Double calcularSalarioMensual(double salarioContratacion, int pagoLey8114)
        {
            double salarioMensual = salarioContratacion + pagoLey8114;
            return salarioMensual;
        }

        /// <summary>
        /// Jean Carlos Monge Mendez
        /// 11/09/2019
        /// Efecto : Calcula el promedio del salario mensual de los dos semestres
        /// Requiere : Salario mensual primer semestre, salario mensual segundo semestre
        /// Modifica : -
        /// Devuelve : Promedio de los dos semetres
        /// </summary>
        /// <param name="salarioPrimerSemestre"></param>
        /// <param name="salarioSegundoSemestre"></param>
        /// <returns></returns>
        private Double calcularPromedioSemestres(double salarioPrimerSemestre, double salarioSegundoSemestre)
        {
            double promedio = (salarioPrimerSemestre + salarioSegundoSemestre) / 2;
            return promedio;
        }

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
        /// 25/09/2019
        /// Efecto : Deja todos los campos del formulario en blanco
        /// Requiere : -
        /// Modifica : Formulario de editar y agregar
        /// Devuelve : -
        /// </summary>
        private void limpiarFormulario()
        {
            txtEscalafonesI.Text = "";
            txtEscalafonesII.Text = "";
            txtMontoAnualidadesI.Text = "";
            txtMontoAnualidadesII.Text = "";
            txtMontoEscalafonesI.Text = "";
            txtMontoEscalafonesII.Text = "";
            txtPorcentajeAnualidadesI.Text = "";
            txtPorcentajeAnualidadesII.Text = "";
            txtPromedioSemestres.Text = "";
            txtSalarioBase1.Text = "";
            txtSalarioBase2.Text = "";
            txtSalarioMensualEneroJunio.Text = "";
            txtSalarioMensualJunioDiciembre.Text = "";
            txtSalContratacionI.Text = "";
            txtSalContratacionII.Text = "";
            txtSumaTotalSalarioBase1.Text = "";
            txtSumaTotalSalarioBaseII.Text = "";
            txtObservaciones.Text = "";
            txtPagoLey8114.Text = "";
            txtSumaSalarioBase1.Text = "";
            txtSalarioPropuesto.Text = "";
        }
        #endregion

        #region eventos

        #region paginacion
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
        /// 16/jul/2019
        /// Efecto: levanta el modal para crear un funcionario y agregarlo a la planilla seleccionada
        /// Requiere: dar clic en el boton de "Nuevo funcionario"
        /// Modifica: -
        /// Devuelve: -
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnNuevoFuncionario_Click(object sender, EventArgs e)
        {
            List<EscalaSalarial> listaEscalas = (List<EscalaSalarial>)Session["listaEscalas"];
            EscalaSalarial escalaSeleccionadaModal = new EscalaSalarial();
            escalaSeleccionadaModal.idEscalaSalarial = Convert.ToInt32(ddlEscalaSalarial.SelectedValue);
            foreach (EscalaSalarial escalaSalarial in listaEscalas)
            {
                if (escalaSalarial.idEscalaSalarial == escalaSeleccionadaModal.idEscalaSalarial)
                {
                    escalaSeleccionada = escalaSalarial;
                    txtSalarioBase1.Text = escalaSalarial.salarioBase1.ToString();
                    txtSalarioBase2.Text = escalaSalarial.salarioBase2.ToString();
                }
            }
            tituloModalFuncionario.InnerText = "Nuevo Funcionario";
            btnGuardar.Text = "Guardar";
            txtFecha.Text = "";
            txtNombreCompleto.Text = "";
            hdIdFuncionario.Value = "0";
            limpiarFormulario();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "activar", "activarModalNuevoFuncionario();", true);
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
            String url = Page.ResolveUrl("~/Planilla/AdministrarPlanilla.aspx");
            Response.Redirect(url);
        }

        /// <summary>
        /// Leonardo Carrion
        /// 17/jul/2019
        /// Efecto: cambia los salarios bases del modal de nuevo funcionario
        /// Requiere: cambiar escala salarial
        /// Modifica: salarios base 1 y 2
        /// Devuelve: -
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlEscalaSalarial_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<EscalaSalarial> listaEscalas = (List<EscalaSalarial>)Session["listaEscalas"];

            EscalaSalarial escalaSeleccionadaModal = new EscalaSalarial();
            escalaSeleccionadaModal.idEscalaSalarial = Convert.ToInt32(ddlEscalaSalarial.SelectedValue);

            foreach (EscalaSalarial escalaSalarial in listaEscalas)
            {
                if (escalaSalarial.idEscalaSalarial == escalaSeleccionadaModal.idEscalaSalarial)
                {
                    limpiarFormulario();
                    escalaSeleccionada = escalaSalarial;
                    txtSalarioBase1.Text = escalaSalarial.salarioBase1.ToString();
                    txtSalarioBase2.Text = escalaSalarial.salarioBase2.ToString();
                }
            }

            ScriptManager.RegisterStartupScript(this, this.GetType(), "activar", "activarModalNuevoFuncionario();", true);
        }

        /// <summary>
        /// Leonardo Carrion
        /// 17/jul/2019
        /// Efecto: calcula el monto de escalafones segun los datos ingresados
        /// Requiere: ingresar numero de escalafones, escoger escala salarial y darle clic al boton de calcular en monto escalafones
        /// Modifica: monto de escalafones
        /// Devuelve: -
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnCalcularEscalafones_Click(object sender, EventArgs e)
        {
            LinkButton btnEscalafones = (LinkButton)sender;
            double salarioBase = 0;
            int cantidadEscalafones = 0;
            if (btnEscalafones.ID.Equals("btnCalcularEscalafonesI"))
            {
                Double.TryParse(escalaSeleccionada.salarioBase1.ToString(), out salarioBase);
                int.TryParse(txtEscalafonesI.Text, out cantidadEscalafones);
                txtEscalafonesI.Text = cantidadEscalafones.ToString();
                txtMontoEscalafonesI.Text = calcularMontoEscalafon(salarioBase, cantidadEscalafones).ToString();
            }
            else if (btnEscalafones.ID.Equals("btnCalcularEscalafonesII"))
            {
                Double.TryParse(escalaSeleccionada.salarioBase2.ToString(), out salarioBase);
                int.TryParse(txtEscalafonesII.Text, out cantidadEscalafones);
                txtEscalafonesII.Text = cantidadEscalafones.ToString();
                txtMontoEscalafonesII.Text = calcularMontoEscalafon(salarioBase, cantidadEscalafones).ToString();
            }
            ScriptManager.RegisterStartupScript(this, this.GetType(), "activar", "activarModalNuevoFuncionario();", true);
        }

        /// <summary>
        /// Jean Carlos Monge Mendez
        /// 02/09/2019
        /// Efecto : Calcula el salario de contratacion y lo muestra en un campo de texto
        /// Requiere : Salario base, monto de escalafones, monto de anualidad, clickear el boton calcular salario contratacion
        /// Modifica : El campo de texto con el salario de contratacion
        /// Devuelve : -
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnCalcularSalContratacion_Click(object sender, EventArgs e)
        {
            LinkButton btnSalarioContratacion = (LinkButton)sender;
            double sumaSalarioBase = 0;
            double montoEscalafones = 0;
            double montoAnualidad = 0;
            if (btnSalarioContratacion.ID.Equals("btnCalcularSalContratacionI"))
            {
                Double.TryParse(txtSumaTotalSalarioBase1.Text.Replace(".", ","), out sumaSalarioBase);
                Double.TryParse(txtMontoEscalafonesI.Text.Replace(".", ","), out montoEscalafones);
                Double.TryParse(txtMontoAnualidadesI.Text.Replace(".", ","), out montoAnualidad);
                txtSalContratacionI.Text = calcularSalarioContratacion(sumaSalarioBase, montoEscalafones, montoAnualidad).ToString();
                txtSumaTotalSalarioBase1.Text = sumaSalarioBase.ToString();
                txtMontoEscalafonesI.Text = montoEscalafones.ToString();
                txtMontoAnualidadesI.Text = montoAnualidad.ToString();
            }
            else if (btnSalarioContratacion.ID.Equals("btnCalcularSalContratacionII"))
            {
                Double.TryParse(txtSumaTotalSalarioBaseII.Text.Replace(".", ","), out sumaSalarioBase);
                Double.TryParse(txtMontoEscalafonesII.Text.Replace(".", ","), out montoEscalafones);
                Double.TryParse(txtMontoAnualidadesII.Text.Replace(".", ","), out montoAnualidad);
                txtSalContratacionII.Text = calcularSalarioContratacion(sumaSalarioBase, montoEscalafones, montoAnualidad).ToString();
                txtSumaTotalSalarioBaseII.Text = sumaSalarioBase.ToString();
                txtMontoEscalafonesII.Text = montoEscalafones.ToString();
                txtMontoAnualidadesII.Text = montoAnualidad.ToString();
            }
            ScriptManager.RegisterStartupScript(this, this.GetType(), "activar", "activarModalNuevoFuncionario();", true);
        }

        /// <summary>
        /// Jean Carlos Monge Mendez
        /// 04/09/2019
        /// Efecto: Calcula el salario mensual a proponer real Ene-Jun
        /// Requiere : Salario de contratacion, Concepto de pago Ley 8114, clickear el boton calcular salario mensual
        /// Modifica : Campo de texto con el salario mensual 
        /// Devuelve : -
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnCalcularSalarioMensualPorSemestre_Click(object sender, EventArgs e)
        {
            LinkButton btnCalcularSalario = (LinkButton)sender;
            double salarioContratacion = 0;
            int pagoLey8114 = 0;
            if (btnCalcularSalario.ID.Equals("btnCalcularSalarioMensualI"))
            {
                Double.TryParse(txtSalContratacionI.Text.Replace(".", ","), out salarioContratacion);
                int.TryParse(txtPagoLey8114.Text, out pagoLey8114);
                txtSalarioMensualEneroJunio.Text = calcularSalarioMensual(salarioContratacion, pagoLey8114).ToString();
                txtSalContratacionI.Text = salarioContratacion.ToString();
                txtPagoLey8114.Text = pagoLey8114.ToString();
            }
            else if (btnCalcularSalario.ID.Equals("btnCalcularSalarioMensualII"))
            {
                Double.TryParse(txtSalContratacionII.Text.Replace(".", ","), out salarioContratacion);
                int.TryParse(txtPagoLey8114.Text, out pagoLey8114);
                txtSalarioMensualJunioDiciembre.Text = calcularSalarioMensual(salarioContratacion, pagoLey8114).ToString();
                txtSalContratacionII.Text = salarioContratacion.ToString();
                txtPagoLey8114.Text = pagoLey8114.ToString();
            }
            ScriptManager.RegisterStartupScript(this, this.GetType(), "activar", "activarModalNuevoFuncionario();", true);
        }

        /// <summary>
        /// Leonardo Carrion
        /// 17/jul/2019
        /// Efecto: calcula el monto de la anualidad segun los datos ingresados
        /// Requiere: monto salario base, monto escalafon y porcentaje anualidad, clickear el boton calcular
        /// Modifica: el campo con el monto de anualidad
        /// Devuelve: -
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnCalcularMontoAnualidades_Click(object sender, EventArgs e)
        {
            LinkButton bntMontoAnualidades = (LinkButton)sender;
            double salarioBase = 0;
            double montoEscalafones = 0;
            double porcentajeAnualidad = 0;

            if (bntMontoAnualidades.ID.Equals("btnCalcularMontoAnualidadesI"))
            {
                Double.TryParse(escalaSeleccionada.salarioBase1.ToString(), out salarioBase);
                Double.TryParse(txtMontoEscalafonesI.Text.Replace(".", ","), out montoEscalafones);
                Double.TryParse(txtPorcentajeAnualidadesI.Text.Replace(".", ","), out porcentajeAnualidad);
                txtMontoAnualidadesI.Text = calcularMontoAnualidad(salarioBase, montoEscalafones, porcentajeAnualidad).ToString();
                txtMontoEscalafonesI.Text = montoEscalafones.ToString();
                txtPorcentajeAnualidadesI.Text = porcentajeAnualidad.ToString();
            }
            else if (bntMontoAnualidades.ID.Equals("btnCalcularMontoAnualidadesII"))
            {
                Double.TryParse(escalaSeleccionada.salarioBase2.ToString(), out salarioBase);
                Double.TryParse(txtMontoEscalafonesII.Text.Replace(".", ","), out montoEscalafones);
                Double.TryParse(txtPorcentajeAnualidadesII.Text.Replace(".", ","), out porcentajeAnualidad);
                txtMontoAnualidadesII.Text = calcularMontoAnualidad(salarioBase, montoEscalafones, porcentajeAnualidad).ToString();
                txtMontoEscalafonesII.Text = montoEscalafones.ToString();
                txtPorcentajeAnualidadesII.Text = porcentajeAnualidad.ToString();
            }
            ScriptManager.RegisterStartupScript(this, this.GetType(), "activar", "activarModalNuevoFuncionario();", true);
        }

        /// <summary>
        /// Jean Carlos Monge Mendez
        /// 11/09/2019
        /// Efecto : Calcula el numero de escalafones para el segundo semestre
        /// Requiere : Escalafones primer semestre, fecha de ingreso
        /// Modifica : Campo de texto escalafonesII
        /// Devuelve : 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnCalcularNumeroEscalafones2_Click(object sender, EventArgs e)
        {
            int cantidadEscalafonesIsemestre = 0;
            DateTime fechaIngreso = DateTime.Now;

            int.TryParse(txtEscalafonesI.Text, out cantidadEscalafonesIsemestre);
            DateTime.TryParse(txtFecha.Text, out fechaIngreso);
            DateTime fechaDivisionSemestres = Convert.ToDateTime("1/06/" + fechaIngreso.Year);
            if (cantidadEscalafonesIsemestre < escalaSeleccionada.topeEscalafones && DateTime.Compare(fechaDivisionSemestres, fechaIngreso) < 0)
            {
                txtEscalafonesII.Text = (cantidadEscalafonesIsemestre + 1).ToString();
            }
            else
            {
                txtEscalafonesII.Text = cantidadEscalafonesIsemestre.ToString();
            }
            ScriptManager.RegisterStartupScript(this, this.GetType(), "activar", "activarModalNuevoFuncionario();", true);
        }

        /// <summary>
        /// Jean Carlos Monge Mendez
        /// 11/09/2019
        /// Efecto : Calcular el promedio de los salarios semestrales y lo muestra en un campo de texto
        /// Requiere: salario mensual primer semestre, salario mensual segundo semestre, clickear el boton calcular promedio
        /// Mofidica: campo de texto promedio dos semetres
        /// Devuelve : -
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnCalcularPromedioSemestres_Click(object sender, EventArgs e)
        {
            double primerSemestre = 0;
            double segundoSemestre = 0;
            Double.TryParse(txtSalarioMensualEneroJunio.Text.Replace(".", ","), out primerSemestre);
            Double.TryParse(txtSalarioMensualJunioDiciembre.Text.Replace(".", ","), out segundoSemestre);
            txtPromedioSemestres.Text = calcularPromedioSemestres(primerSemestre, segundoSemestre).ToString();
            txtSalarioMensualEneroJunio.Text = primerSemestre.ToString();
            txtSalarioMensualJunioDiciembre.Text = segundoSemestre.ToString();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "activar", "activarModalNuevoFuncionario();", true);
        }

        /// <summary>
        /// Jean Carlos Monge Mendez
        /// 06/09/2019
        /// Efecto : Calcula el salario total del funcionario
        /// Requiere : Salario base, Suma al salario base, clickear el boton de calcular total salario base 
        /// Modifica : Campo de texto con el valor del salario total
        /// Devuelve : -
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnCalcularTotalSalarioBase_Click(object sender, EventArgs e)
        {
            LinkButton btnTotalSalarioBase = (LinkButton)sender;
            double salarioBase = 0;
            double sumaSalarioBase = 0;
            Double.TryParse(txtSumaSalarioBase1.Text.Replace(".", ",").ToString(), out sumaSalarioBase);
            if (btnTotalSalarioBase.ID.Equals("btnCalcularTotalSalarioBaseI"))
            {
                salarioBase = escalaSeleccionada.salarioBase1;
                txtSumaTotalSalarioBase1.Text = calcularTotalSalarioBase(salarioBase, sumaSalarioBase).ToString();
            }
            else if (btnTotalSalarioBase.ID.Equals("btnCalcularTotalSalarioBaseII"))
            {
                salarioBase = escalaSeleccionada.salarioBase2;
                txtSumaTotalSalarioBaseII.Text = calcularTotalSalarioBase(salarioBase, sumaSalarioBase).ToString();
            }
            txtSumaSalarioBase1.Text = sumaSalarioBase.ToString();

            ScriptManager.RegisterStartupScript(this, this.GetType(), "activar", "activarModalNuevoFuncionario();", true);
        }

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
        protected void btnSelccionar_Click(object sender, EventArgs e)
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
            txtVerNombreCompleto.Text = funcionarioVer.nombreFuncionario;
            txtVerEscalafonesI.Text = funcionarioVer.noEscalafones1.ToString();
            txtVerEscalafonesII.Text = funcionarioVer.noEscalafones2.ToString();
            txtVerEscalaSalarial.Text = funcionarioVer.escalaSalarial.descEscalaSalarial;
            txtVerFecha.Text = funcionarioVer.fechaIngreso.ToString();
            txtVerMontoAnualidadesI.Text = funcionarioVer.montoAnualidad1.ToString();
            txtVerMontoAnualidadesII.Text = funcionarioVer.montoAnualidad2.ToString();
            txtVerMontoEscalafonesI.Text = funcionarioVer.montoEscalafones1.ToString();
            txtVerMontoEscalafonesII.Text = funcionarioVer.montoEscalafones2.ToString();
            txtVerObservaciones.Text = funcionarioVer.observaciones;
            txtVerPagoLey8114.Text = funcionarioVer.conceptoPagoLey.ToString();
            txtVerPorcentajeAnualidadesI.Text = funcionarioVer.porcentajeAnualidad1.ToString();
            txtVerPorcentajeAnualidadesII.Text = funcionarioVer.porcentajeAnualidad2.ToString();
            txtVerPromedioSemestres.Text = funcionarioVer.salarioPromedio.ToString();
            txtVerSalarioBaseI.Text = funcionarioVer.escalaSalarial.salarioBase1.ToString();
            txtVerSalarioBase2.Text = funcionarioVer.escalaSalarial.salarioBase2.ToString();
            txtVerSalarioMensualEneroJunio.Text = funcionarioVer.salarioEnero.ToString();
            txtVerSalarioMensualJunioDiciembre.Text = funcionarioVer.salarioJunio.ToString();
            txtVerSalarioPropuesto.Text = funcionarioVer.salarioPropuesto.ToString();
            txtVerSalContratacionI.Text = funcionarioVer.salarioContratacion1.ToString();
            txtVerSalContratacionII.Text = funcionarioVer.salarioContratacion2.ToString();
            txtVerSumaSalarioBase1.Text = funcionarioVer.porcentajeSumaSalario.ToString();
            txtVerSumaSalarioBase2.Text = funcionarioVer.porcentajeSumaSalario.ToString();
            txtVerSumaTotalSalarioBase1.Text = funcionarioVer.salarioBase1.ToString();
            txtVerSumaTotalSalarioBaseII.Text = funcionarioVer.salarioBase2.ToString();
            btnConfirmarEliminar.Visible = false;
            tituloVerFuncionario.InnerText = "Ver Funcionario";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "activar", "activarModalVerFuncionario();", true);
        }

        /// <summary>
        /// Jean Carlos Monge Mendez
        /// 25/09/2019
        /// Efecto : Muestra en un modal los datos de un funcionario que se desea modificar
        /// Requiere : Clickear el boton "Editar" de un funcionario 
        /// Modifica : Formulario de editar
        /// Devuelve : -
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnEditar_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32((((LinkButton)(sender)).CommandArgument).ToString());
            Funcionario funcionarioEditar = null;
            List<Funcionario> funcionarios = (List<Funcionario>)Session["listaFuncionariosFiltrada"];
            foreach (Funcionario funcionario in funcionarios)
            {
                if (funcionario.idFuncionario == id)
                {
                    funcionarioEditar = funcionario;
                    break;
                }
            }
            tituloModalFuncionario.InnerText = "Editar Funcionario";
            btnGuardar.Text = "Actualizar";
            txtNombreCompleto.Text = funcionarioEditar.nombreFuncionario;
            txtEscalafonesI.Text = funcionarioEditar.noEscalafones1.ToString();
            txtEscalafonesII.Text = funcionarioEditar.noEscalafones2.ToString();
            ddlEscalaSalarial.SelectedValue = funcionarioEditar.escalaSalarial.idEscalaSalarial.ToString();
            txtFecha.Text = funcionarioEditar.fechaIngreso.ToString();
            txtMontoAnualidadesI.Text = funcionarioEditar.montoAnualidad1.ToString();
            txtMontoAnualidadesII.Text = funcionarioEditar.montoAnualidad2.ToString();
            txtMontoEscalafonesI.Text = funcionarioEditar.montoEscalafones1.ToString();
            txtMontoEscalafonesII.Text = funcionarioEditar.montoEscalafones2.ToString();
            txtObservaciones.Text = funcionarioEditar.observaciones;
            txtPagoLey8114.Text = funcionarioEditar.conceptoPagoLey.ToString();
            txtPorcentajeAnualidadesI.Text = funcionarioEditar.porcentajeAnualidad1.ToString();
            txtPorcentajeAnualidadesII.Text = funcionarioEditar.porcentajeAnualidad2.ToString();
            txtPromedioSemestres.Text = funcionarioEditar.salarioPromedio.ToString();
            txtSalarioBase1.Text = funcionarioEditar.escalaSalarial.salarioBase1.ToString();
            txtSalarioBase2.Text = funcionarioEditar.escalaSalarial.salarioBase2.ToString();
            txtSalarioMensualEneroJunio.Text = funcionarioEditar.salarioEnero.ToString();
            txtSalarioMensualJunioDiciembre.Text = funcionarioEditar.salarioJunio.ToString();
            txtSalarioPropuesto.Text = funcionarioEditar.salarioPropuesto.ToString();
            txtSalContratacionI.Text = funcionarioEditar.salarioContratacion1.ToString();
            txtSalContratacionII.Text = funcionarioEditar.salarioContratacion2.ToString();
            txtSumaSalarioBase1.Text = funcionarioEditar.porcentajeSumaSalario.ToString();
            txtSumaTotalSalarioBase1.Text = funcionarioEditar.salarioBase1.ToString();
            txtSumaTotalSalarioBaseII.Text = funcionarioEditar.salarioBase2.ToString();
            hdIdFuncionario.Value = funcionarioEditar.idFuncionario.ToString();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "activar", "activarModalNuevoFuncionario();", true);
        }

        /// <summary>
        /// Jean Carlos Monge Mendez
        /// 25/09/2019
        /// Efecto : Muestra en un modal los datos de un funcionario que se desea eliminar
        /// Requiere : Clickear el boton "Eliminar" de un funcionario
        /// Modifica : Modal Ver
        /// Devuelve : -
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnEliminar_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32((((LinkButton)(sender)).CommandArgument).ToString());
            Funcionario funcionarioEliminar = null;
            List<Funcionario> funcionarios = (List<Funcionario>)Session["listaFuncionariosFiltrada"];
            foreach (Funcionario funcionario in funcionarios)
            {
                if (funcionario.idFuncionario == id)
                {
                    funcionarioEliminar = funcionario;
                    break;
                }
            }
            txtVerNombreCompleto.Text = funcionarioEliminar.nombreFuncionario;
            txtVerEscalafonesI.Text = funcionarioEliminar.noEscalafones1.ToString();
            txtVerEscalafonesII.Text = funcionarioEliminar.noEscalafones2.ToString();
            txtVerEscalaSalarial.Text = funcionarioEliminar.escalaSalarial.descEscalaSalarial;
            txtVerFecha.Text = funcionarioEliminar.fechaIngreso.ToString();
            txtVerMontoAnualidadesI.Text = funcionarioEliminar.montoAnualidad1.ToString();
            txtVerMontoAnualidadesII.Text = funcionarioEliminar.montoAnualidad2.ToString();
            txtVerMontoEscalafonesI.Text = funcionarioEliminar.montoEscalafones1.ToString();
            txtVerMontoEscalafonesII.Text = funcionarioEliminar.montoEscalafones2.ToString();
            txtVerObservaciones.Text = funcionarioEliminar.observaciones;
            txtVerPagoLey8114.Text = funcionarioEliminar.conceptoPagoLey.ToString();
            txtVerPorcentajeAnualidadesI.Text = funcionarioEliminar.porcentajeAnualidad1.ToString();
            txtVerPorcentajeAnualidadesII.Text = funcionarioEliminar.porcentajeAnualidad2.ToString();
            txtVerPromedioSemestres.Text = funcionarioEliminar.salarioPromedio.ToString();
            txtVerSalarioBaseI.Text = funcionarioEliminar.escalaSalarial.salarioBase1.ToString();
            txtVerSalarioBase2.Text = funcionarioEliminar.escalaSalarial.salarioBase2.ToString();
            txtVerSalarioMensualEneroJunio.Text = funcionarioEliminar.salarioEnero.ToString();
            txtVerSalarioMensualJunioDiciembre.Text = funcionarioEliminar.salarioJunio.ToString();
            txtVerSalarioPropuesto.Text = funcionarioEliminar.salarioPropuesto.ToString();
            txtVerSalContratacionI.Text = funcionarioEliminar.salarioContratacion1.ToString();
            txtVerSalContratacionII.Text = funcionarioEliminar.salarioContratacion2.ToString();
            txtVerSumaSalarioBase1.Text = funcionarioEliminar.porcentajeSumaSalario.ToString();
            txtVerSumaSalarioBase2.Text = funcionarioEliminar.porcentajeSumaSalario.ToString();
            txtVerSumaTotalSalarioBase1.Text = funcionarioEliminar.salarioBase1.ToString();
            txtVerSumaTotalSalarioBaseII.Text = funcionarioEliminar.salarioBase2.ToString();
            tituloVerFuncionario.InnerText = "Eliminar Funcionario";
            btnConfirmarEliminar.Visible = true;
            hdIdEliminarFuncionario.Value = funcionarioEliminar.idFuncionario.ToString();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "activar", "activarModalVerFuncionario();", true);
        }

        /// <summary>
        /// Jean Carlos Monge Mendez
        /// 25/09/2019
        /// Efecto : Elimina un funcionario 
        /// Requiere : Clickear el  boton "Eliminar" del modal
        /// Modifica : -
        /// Devuelve : -
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnConfirmarEliminar_Click(object sender, EventArgs e)
        {
            Funcionario funcionario = new Funcionario();
            int idFuncionarioEliminar = 0;
            Int32.TryParse(hdIdEliminarFuncionario.Value, out idFuncionarioEliminar);
            funcionario.idFuncionario = idFuncionarioEliminar;
            bool result = funcionarioServicios.eliminar(funcionario);
            List<Funcionario> listaFuncionarios = funcionarioServicios.getFuncionarios();
            Session["listaFuncionarios"] = listaFuncionarios;
            Session["listaFuncionariosFiltrada"] = listaFuncionarios;
            mostrarDatosTabla();
        }

        /// <summary>
        /// Jean Carlos Monge Mendez
        /// 18/09/2019
        /// Efecto : Guarda los datos de un funcionario
        /// Requiere : campos validos, clickear el boton "Guardar"
        /// Modifica : -
        /// Devuelve : -
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            bool result = false;
            Funcionario funcionario = new Funcionario();
            funcionario.conceptoPagoLey = Convert.ToDouble(txtPagoLey8114.Text);
            EscalaSalarial escalaSalarial = new EscalaSalarial();
            escalaSalarial.idEscalaSalarial = Convert.ToInt32(ddlEscalaSalarial.SelectedValue);
            funcionario.escalaSalarial = escalaSalarial;
            funcionario.fechaIngreso = Convert.ToDateTime(txtFecha.Text);
            funcionario.montoAnualidad1 = Convert.ToDouble(txtMontoAnualidadesI.Text);
            funcionario.montoAnualidad2 = Convert.ToDouble(txtMontoAnualidadesII.Text);
            funcionario.montoEscalafones1 = Convert.ToDouble(txtMontoEscalafonesI.Text);
            funcionario.montoEscalafones2 = Convert.ToDouble(txtMontoEscalafonesII.Text);
            funcionario.noEscalafones1 = Convert.ToInt32(txtEscalafonesI.Text);
            funcionario.noEscalafones2 = Convert.ToInt32(txtEscalafonesII.Text);
            funcionario.nombreFuncionario = txtNombreCompleto.Text;
            funcionario.observaciones = txtObservaciones.Text;
            funcionario.planilla = (Entidades.Planilla)Session["planillaSeleccionada"];
            funcionario.porcentajeAnualidad1 = Convert.ToDouble(txtPorcentajeAnualidadesI.Text);
            funcionario.porcentajeAnualidad2 = Convert.ToDouble(txtPorcentajeAnualidadesII.Text);
            funcionario.salarioBase1 = Convert.ToDouble(txtSumaTotalSalarioBase1.Text);
            funcionario.salarioBase2 = Convert.ToDouble(txtSumaTotalSalarioBaseII.Text);
            funcionario.salarioContratacion1 = Convert.ToDouble(txtSalContratacionI.Text);
            funcionario.salarioContratacion2 = Convert.ToDouble(txtSalContratacionII.Text);
            funcionario.salarioEnero = Convert.ToDouble(txtSalarioMensualEneroJunio.Text);
            funcionario.salarioJunio = Convert.ToDouble(txtSalarioMensualJunioDiciembre.Text);
            funcionario.salarioPromedio = Convert.ToDouble(txtPromedioSemestres.Text);
            funcionario.porcentajeSumaSalario = Convert.ToDouble(txtSumaSalarioBase1.Text);
            double salarioPropuesto = 0;
            Double.TryParse(txtSalarioPropuesto.Text, out salarioPropuesto);
            funcionario.salarioPropuesto = salarioPropuesto;
            int idFuncionario = 0;
            Int32.TryParse(hdIdFuncionario.Value, out idFuncionario);
            funcionario.idFuncionario = idFuncionario;
            if (idFuncionario > 0)
            {
                result = funcionarioServicios.modificar(funcionario);
            }
            else
            {
                result = funcionarioServicios.guardar(funcionario);
            }
            List<Funcionario> listaFuncionarios = funcionarioServicios.getFuncionarios();
            Session["listaFuncionarios"] = listaFuncionarios;
            Session["listaFuncionariosFiltrada"] = listaFuncionarios;
            mostrarDatosTabla();
        }
        #endregion
    }
}