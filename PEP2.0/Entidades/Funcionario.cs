﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    /// <summary>
    /// Leonardo Carrion
    /// 12/jul/2019
    /// Clase para administrar la entidad de funcionario
    /// </summary>
    public class Funcionario
    {
        public int idFuncionario { get; set; }
        public Planilla planilla { get; set; }
        public EscalaSalarial escalaSalarial { get; set; }
        public DateTime fechaIngreso { get; set; }
        public Double salarioBase1 { get; set; }
        public int noEscalafones1 { get; set; }
        public Double montoEscalafones1 { get; set; }
        public Double porcentajeAnualidad1 { get; set; }
        public Double montoAnualidad1 { get; set; }
        public Double salarioContratacion1 { get; set; }
        public Double salarioEnero { get; set; }
        public Double conceptoPagoLey { get; set; }
        public Double salarioBase2 { get; set; }
        public int noEscalafones2 { get; set; }
        public Double montoEscalafones2 { get; set; }
        public Double porcentajeAnualidad2 { get; set; }
        public Double montoAnualidad2 { get; set; }
        public Double salarioContratacion2 { get; set; }
        public Double salarioJunio { get; set; }
        public Double salarioPromedio { get; set; }
        public String observaciones { get; set; }
        public String nombreFuncionario { get; set; }
        public double porcentajeSumaSalario { get; set; }
        public Jornada JornadaLaboral { get; set; }

        /// <summary>
        /// Leonardo Carrion
        /// 06/nov/2019
        /// para poder mostrar cuales funcionarios les hace falta distribuir en la pantalla de administrar distribuccion
        /// </summary>
        public Double porcentajeAsignado { get; set; }
    }
}
