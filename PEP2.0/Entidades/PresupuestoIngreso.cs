﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class PresupuestoIngreso
    {
        public int idPresupuestoIngreso { get; set; }
        public Double monto { get; set; }
        public Boolean esInicial { get; set; }
        public Proyectos proyecto { get; set; }
        public EstadoPresupIngreso estadoPresupIngreso { get; set; }

    }
}
