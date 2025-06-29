using System;
using System.Collections.Generic;

namespace SeguridadService.Domain.Entity;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public string Nombre { get; set; } = null!;

    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public bool? Activo { get; set; }

    public short UsuarioCreacion { get; set; }

    public DateTime FechaCreacion { get; set; }

    public short? UsuarioModificacion { get; set; }

    public DateTime? FechaModificacion { get; set; }
}
