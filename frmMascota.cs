using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;




namespace TUP_PI_Recu_Mascotas
{
    public partial class frmMascota : Form
    {
        private SqlConnection conexion;
        private SqlCommand comando;
        public frmMascota()
        {
            InitializeComponent();
          
        }

        private void frmMascota_Load(object sender, EventArgs e)
        {
            conexion = new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=Veterinaria;Integrated Security=True");
            cargarcombo();
            cargarLista();
            controles(true);
        }

        private void controles(bool value)
        {
            btnNuevo.Enabled = value;
            btnSalir.Enabled = value;
            btnGrabar.Enabled = !value;
            txtCodigo.Enabled = !value;
            txtNombre.Enabled = !value;
            cboEspecie.Enabled = !value;
            rbtHembra.Enabled = !value;
            rbtMacho.Enabled = !value;
            dtpFechaNacimiento.Enabled = !value;
            lstMascotas.Enabled = !value;
                
        }

        private void cargarLista()
        {
            try
            {
                conexion.Open();
                DataTable tabla = new DataTable();
                comando = new SqlCommand();
                comando.CommandText = "SELECT (str(codigo) + ' -- ' + nombre) as datos, codigo FROM Mascotas";
                comando.Connection = conexion;
                tabla.Load(comando.ExecuteReader());
                conexion.Close();

                lstMascotas.DataSource = tabla;
                lstMascotas.ValueMember = "codigo";
                lstMascotas.DisplayMember = "datos";
            }
            catch (Exception) { }
        }

        private void cargarcombo()
        {
            try
            {
                conexion.Open();
                DataTable tabla = new DataTable();
                comando = new SqlCommand();
                comando.CommandText = "SELECT * FROM Especies";
                comando.Connection = conexion;
                tabla.Load(comando.ExecuteReader());
                conexion.Close();

                cboEspecie.DataSource = tabla;
                cboEspecie.ValueMember = tabla.Columns[0].ColumnName;
                cboEspecie.DisplayMember = tabla.Columns[1].ColumnName;
            }
            catch (Exception) 
            {
            }
        }

        private void btnGrabar_Click(object sender, EventArgs e)
        {
            try { 
                if (String.IsNullOrEmpty(txtCodigo.Text.Trim()))
                {
                    MessageBox.Show("Campo obligatorio!");
                    txtCodigo.Focus();
                    return;
                }
                if (txtNombre.Text.Equals(""))
                {
                    MessageBox.Show("Debe ingresar un nombre");
                    txtNombre.Focus();
                    return;
                }
                if(cboEspecie.SelectedIndex == -1)
                {
                    MessageBox.Show("Debe seleccionar una mascota");
                    cboEspecie.Focus();
                    return;
                }
                int codigo;
                try
                {
                    codigo = Convert.ToInt32(txtCodigo.Text);
                }
                catch (Exception)
                {
                    MessageBox.Show("Codigo Incorrecto!");
                    return;
                }
            
                Mascota nueva = new Mascota();
                nueva.pCodigo = Convert.ToInt32(txtCodigo.Text);
                nueva.pNombre = txtNombre.Text;
                nueva.pEspecie = cboEspecie.SelectedIndex;
                if (rbtMacho.Checked)
                    nueva.pSexo = 1;
                else
                    nueva.pSexo = 2;
                nueva.pFechaNacimiento = dtpFechaNacimiento.Value;

                conexion = new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=Veterinaria;Integrated Security=True");
           
                conexion.Open();
                comando = new SqlCommand();
                comando.Connection = conexion;
                comando.CommandText = "INSERT INTO Mascotas (codigo,nombre,especie,sexo,fechaNacimiento)" +
                    "VALUES (@codigo, @nombre, @especie, @sexo, @fechaNacimiento)";
                comando.Parameters.AddWithValue("codigo", nueva.pCodigo);
                comando.Parameters.AddWithValue("nombre", nueva.pNombre);
                comando.Parameters.AddWithValue("especie", nueva.pEspecie);
                comando.Parameters.AddWithValue("sexo", nueva.pSexo);
                comando.Parameters.AddWithValue("fechaNacimiento", nueva.pFechaNacimiento);
                comando.ExecuteNonQuery();
                conexion.Close();
                MessageBox.Show("Mascota cargada correctamente");
                cargarLista();
                controles(true);
                limpiarCampos();
            }
            catch (Exception)
            {
                MessageBox.Show("No se pudo cargar la mascota ");
            }
        }

        private void limpiarCampos()
        {
            txtCodigo.Text = txtNombre.Text = string.Empty;
            rbtHembra.Checked = false;
            rbtMacho.Checked = false;
            cboEspecie.SelectedIndex = -1;
            dtpFechaNacimiento.Value = DateTime.Now;
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            controles(false);
            txtCodigo.Focus();
            limpiarCampos();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Seguro desea salir la aplicación ?",
                "SALIR", MessageBoxButtons.YesNo, 
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2) == 
                DialogResult.Yes)
                this.Close();
        }
    }
}

