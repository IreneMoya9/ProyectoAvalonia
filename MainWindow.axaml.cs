using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Media;
using Avalonia.Media.Imaging;


namespace AvaloniaApplication4;

public partial class MainWindow : Window
{
    
    
    // Lista que almacena objetos Cliente
    List<Cliente> listaClientes = new List<Cliente>();

    int indice = 0;
    bool esNuevo = false;
    bool esModificar = false;
    private string imagenselect;

    // OpenFileDialog para seleccionar imágenes
    OpenFileDialog examinar = new OpenFileDialog();
    
    public MainWindow()
    {
        
        InitializeComponent();
        CreacionClientesEjemplo();
        CargarRegistros();
        visibilidadBotones(true);
        habilitarEdicion(false);
        TxtPrecio.IsEnabled = false;
        BtnAnterior.IsEnabled = false;
        TxtRegistros.IsEnabled = false;
        MostarActual();

        this.Closing += GuardarDatosCerrar;







    }
    private void CreacionClientesEjemplo()
    {
        Cliente cli1, cli2, cli3;
        string ruta1 = "Imagenes/imagen1.jpg";
        string ruta2 = "Imagenes/imagen2.jpg";
        string ruta3 = "Imagenes/imagen3.jpg";

        byte[] imagen1 = File.ReadAllBytes(ruta1);
        byte[] imagen2 = File.ReadAllBytes(ruta2);
        byte[] imagen3 = File.ReadAllBytes(ruta3);

        cli1 = new Cliente("Sonia", "Sánchez", 123456789, 'F', true, true, imagen1, 25.5f);
        cli2 = new Cliente("Luis", "Rodriguez", 987654321, 'M', true, false, imagen2, 18.7f);
        cli3 = new Cliente("Maria", "Salas", 654789123, 'F', false, true, imagen3, 19.00f);
        
        listaClientes.Add(cli1);
        listaClientes.Add(cli2);
        listaClientes.Add(cli3);
    }
    
    
    private void CargarRegistros()
    {
        // Lectura de datos desde el archivo databank.data
        using (BinaryReader reader = new BinaryReader(File.OpenRead("databank.data")))
        {
            try
            {
                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    string nombre;
                    string apellido;
                    int telefono;
                    char genero;
                    bool tipo1;
                    bool tipo2;
                    byte[] imagen;
                    float precio;

                    nombre = reader.ReadString();
                    apellido = reader.ReadString();
                    telefono = reader.ReadInt32();
                    genero = reader.ReadChar();
                    tipo1 = reader.ReadBoolean();
                    tipo2 = reader.ReadBoolean();
                    int numbyte = reader.ReadInt32();
                    imagen = reader.ReadBytes(numbyte);
                    precio = reader.ReadSingle();

                    Cliente cli = new Cliente(nombre, apellido, telefono, genero, tipo1, tipo2, imagen, precio);
                    listaClientes.Add(cli);
                }
            }
            catch (IOException ex)
            {
                // Manejar la excepción de entrada/salida (IOException) aquí.
                Console.WriteLine($"Error de entrada/salida al leer la imagen: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Manejar otras excepciones aquí.
                Console.WriteLine($"Error al leer la imagen: {ex.Message}");
            }

        }

    }
    
    
    
    public void MostarActual()
    {
        Cliente actual = listaClientes[indice];
        TxtNombre.Text = actual.nombre;
        TxtApellido.Text = actual.apellido;
        TxtTelefono.Text = "" + actual.telefono;
        TxtGenero.Text = "" + actual.genero;
        CbManicura.IsChecked = actual.tipo1;
        CbPintar.IsChecked = actual.tipo2;
        TxtPrecio.Text = string.Format("{0:G}", actual.precio);
        TxtRegistros.Text = Convert.ToString(listaClientes.Count);
        
        using (MemoryStream stream = new MemoryStream(actual.imagen))
        {
            Bitmap bitmap = new Bitmap(stream);
            ImagenUnas.Source = bitmap;
        }
    }
    
    
    
    
    private void GuardarDatosCerrar(object sender, CancelEventArgs e)
    {
        GuardarDatos();
    }
   
    public void GuardarDatos()
    {
        using (BinaryWriter writer = new BinaryWriter(File.OpenWrite("databank.data")))
        {
            foreach (Cliente cli in listaClientes)
            {
                // Escribir datos de clientes en el archivo binario
                writer.Write(cli.nombre);
                writer.Write(cli.apellido);
                writer.Write(cli.telefono);
                writer.Write(cli.genero);
                writer.Write(cli.tipo1);
                writer.Write(cli.tipo2);
                writer.Write(cli.imagen.Length);
                writer.Write(cli.imagen);
                writer.Write(cli.precio);
                
                Console.WriteLine("Datos guardados en el archivo");
            }
        }
    }
    
    private void CbManicura_CheckedChanged(object? sender, RoutedEventArgs e)
    {
        ActualizarPrecio();
    }

    private void CbPedicura_CheckedChanged(object? sender, RoutedEventArgs e)
    {
        ActualizarPrecio();
    }


    private void FunAnterior(object? sender, RoutedEventArgs e)
    {
        indice--;
        BtnAnterior.IsEnabled = true;
        BtnSiguiente.IsEnabled = true;
        if (indice == 0)
        {
            BtnAnterior.IsEnabled = false;
            BtnSiguiente.IsEnabled = false;
        }

        MostarActual();

    }

    private void FunSiguiente(object? sender, RoutedEventArgs e)
    {
        indice++;
        BtnAnterior.IsEnabled = true;
        BtnSiguiente.IsEnabled = true;
        if (indice == listaClientes.Count - 1)
        {
            BtnAnterior.IsEnabled = true;
            BtnSiguiente.IsEnabled = false;
        }

        MostarActual();
    }

    private void FunGuardar(object? sender, RoutedEventArgs e)
    {
        GuardarDatos();
    }

    private void FunModificar(object? sender, RoutedEventArgs e)
    {
        visibilidadBotones(false);
        habilitarEdicion(true);
        esModificar = true;
    }

    private void FunEliminar(object? sender, RoutedEventArgs e)
    {
        listaClientes.RemoveAt(indice);
        if (indice == 0)
        {
            BtnSiguiente.IsEnabled = true;
            BtnAnterior.IsEnabled = false;
        }
        if (indice == listaClientes.Count - 1)
        {
            BtnSiguiente.IsEnabled = false;
            BtnAnterior.IsEnabled= true;
        }
        if (indice == listaClientes.Count)
        {
            indice--;
        }
        MostarActual();
    }

    private void FunNuevo(object? sender, RoutedEventArgs e)
    {
        visibilidadBotones(false);
        habilitarEdicion(true);
        esNuevo = true;

        TxtNombre.Text = "";
        TxtApellido.Text = "";
        TxtTelefono.Text = "";
        TxtGenero.Text = "";
        TxtPrecio.Text = "";
    }

    private void FunAceptar(object? sender, RoutedEventArgs e)
    {
        if (esNuevo)
        {
            string nombre = TxtNombre.Text;
            string apellido = TxtApellido.Text;
            int telefono = int.Parse(TxtTelefono.Text);
            char genero = Convert.ToChar(TxtGenero.Text);
            bool tipo1 = (bool)CbManicura.IsChecked;
            bool tipo2 = (bool)CbPintar.IsChecked;
            byte[] imagen = File.ReadAllBytes(imagenselect);
            ActualizarPrecio();
            float precio = float.Parse(TxtPrecio.Text);

            if (string.IsNullOrWhiteSpace(TxtNombre.Text) || string.IsNullOrWhiteSpace(TxtApellido.Text) ||
                string.IsNullOrWhiteSpace(TxtTelefono.Text) || string.IsNullOrWhiteSpace(TxtGenero.Text) ||
                !float.TryParse(TxtPrecio.Text, out _))
            {
             
                Console.WriteLine("Por favor, complete todos los campos obligatorios correctamente.");
           }
           else
           {
               Cliente nuevoCliente = new Cliente(nombre, apellido, telefono, genero, tipo1, tipo2, imagen, precio);
               listaClientes.Add(nuevoCliente);
               MostarActual();
           }
        }

        if (esModificar)
        {
            if (string.IsNullOrWhiteSpace(TxtNombre.Text) || string.IsNullOrWhiteSpace(TxtApellido.Text) ||
                string.IsNullOrWhiteSpace(TxtTelefono.Text) || string.IsNullOrWhiteSpace(TxtGenero.Text) ||
                !float.TryParse(TxtPrecio.Text, out _))
            {
             
                Console.WriteLine("Por favor, complete todos los campos obligatorios correctamente.");
            }
            else
            {
                
                Cliente modificarCliente = listaClientes[indice];
           
                modificarCliente.nombre = TxtNombre.Text;
                modificarCliente.apellido = TxtApellido.Text;
                modificarCliente.telefono = int.Parse(TxtTelefono.Text);
                modificarCliente.genero = Convert.ToChar(TxtGenero.Text);
                modificarCliente.tipo1 = (bool)CbManicura.IsChecked;
                modificarCliente.tipo2 = (bool)CbPintar.IsChecked;
                modificarCliente.imagen = File.ReadAllBytes(imagenselect);
                ActualizarPrecio();
                modificarCliente.precio = float.Parse(TxtPrecio.Text);

                MostarActual();
            }
            
            
        }
        visibilidadBotones(true);
        habilitarEdicion(false);
        esModificar = false;
        esNuevo = false;
    }

    private void FunCancelar(object? sender, RoutedEventArgs e)
    {
        visibilidadBotones(true);
        habilitarEdicion(false);
        esModificar = false;
        esNuevo = false;
    }
    
    private async void FunExaminarImagen(object? sender, RoutedEventArgs e)
    {
        examinar.Filters.Add(new FileDialogFilter { Name = "JPEG", Extensions = { "jpg" } });

        string[] result = await examinar.ShowAsync(this);

        if (result != null && result.Length > 0)
        {
            try
            {
                imagenselect  = result[0];
                ImagenUnas.Source = new Bitmap(imagenselect);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al cargar la imagen");
            }
        }
        
    }
    
    
    private void visibilidadBotones(bool visible)
    {
        BtnCancelar.IsVisible=!visible;
        BtnAceptar.IsVisible=!visible;
        BtnAnterior.IsVisible = visible;
        BtnEliminar.IsVisible = visible;
        BtnGuardar.IsVisible = visible;
        BtnSiguiente.IsVisible = visible;
        BtnModificar.IsVisible = visible;
        BtnNuevo.IsVisible = visible;
    }

    private void habilitarEdicion(bool editable)
    {
        TxtNombre.IsEnabled = editable;
        TxtApellido.IsEnabled = editable;
        TxtTelefono.IsEnabled = editable;
        TxtGenero.IsEnabled = editable;
        BtnExaminar.IsEnabled = editable;
        CbManicura.IsEnabled = editable;
        CbPintar.IsEnabled = editable;
    }
    
    private void ActualizarPrecio()
    {
        float precio = 3.5f;

        if ((bool)CbManicura.IsChecked)
        {
            precio += 7.3f;
        }

        if ((bool)CbPintar.IsChecked)
        {
            precio += 18.8f;
        }

        TxtPrecio.Text = precio.ToString("0.00");
    }


    
}

internal class Cliente
{
    public string? nombre { get; set; }
    public string? apellido { get; set; }
    public int telefono { get; set; }
    public char genero { get; set; }
    public bool tipo1 { get; set; }
    public bool tipo2 { get; set; }
    public byte[] imagen { get; set; }
    public float precio { get; set; }


    public Cliente(string nombre, string apellido, int telefono, char genero, bool tipo1, bool tipo2,  byte[] imagen,float precio)
    {
        this.nombre = nombre;
        this.apellido = apellido;
        this.telefono = telefono;
        this.genero = genero;
        this.tipo1 = tipo1;
        this.tipo2 = tipo2;
        this.imagen = imagen;
        this.precio = precio;
    }
    
    
}