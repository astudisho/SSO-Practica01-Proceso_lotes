using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace SSO_Practica01_Proceso_lotes
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
			
		public static int segundos = 0;
		public static int tiempoTranscurrido = 0;
		public static int globalMaximo = 0;
		public const int MAX_PROCESOS_LOTE = 5;

		private List<Lote> listaLotes;
		private List<int> listaId;

		private Lote loteActual;
		private Proceso procesoActual;
		private int tiempoMaxEstimado;
        private bool estaCorriendo,
                     estaPausado;

		DispatcherTimer dt;
		Random rnd;

		public MainWindow()
		{
			InitializeComponent();

			cmbOperacion.Items.Add("+");
			cmbOperacion.Items.Add("-");
			cmbOperacion.Items.Add("*");
			cmbOperacion.Items.Add("/");
			cmbOperacion.Items.Add("%");
			cmbOperacion.Items.Add("^");
			cmbOperacion.Items.Add("%%");
			cmbOperacion.SelectedIndex = 0;

			for (int i = 0; i < 15; i++)
			{
				cmbOperando1.Items.Add(i);
				cmbOperando2.Items.Add(i);
			}

			for (int i = 1; i < 30; i++)
			{
				cmbNumeroProcesos.Items.Add(i);
			}

			cmbOperando1.SelectedIndex = 0;
			cmbOperando2.SelectedIndex = 0;
			cmbNumeroProcesos .SelectedIndex = 0;

			dt = new DispatcherTimer();

			dt.Tick += dispatcherTimer_Tick;
			dt.Interval = new System.TimeSpan(0, 0, 1);

			listaLotes = new List<Lote>();
			dgvLotes.ItemsSource = listaLotes;

			listaId = new List<int>();
			rnd = new Random();

			loteActual = new Lote();
			listaLotes.Add(loteActual);
			dgvProcesos.ItemsSource = loteActual.getListaProcesos();

            estaPausado = estaCorriendo = false;
            
			tiempoMaxEstimado = 0;
		}

		private void dispatcherTimer_Tick(object sender, EventArgs e)
		{
			segundos++;

			String tiempo =	(segundos / 60).ToString().PadLeft(2, '0') + ":" 
						  + (segundos % 60).ToString().PadLeft(2,'0');

			txbCronometro.Text = tiempo;

			//Tiempo restante

			var restante = procesoActual.ETA;

			txbRestante.Text = (restante / 60).ToString().PadLeft(2, '0') + ":"
							 + (restante % 60).ToString().PadLeft(2, '0');

			txbTranscurrido.Text = (tiempoTranscurrido / 60).ToString().PadLeft(2, '0') + ":"
									+ (tiempoTranscurrido % 60).ToString().PadLeft(2, '0');

			procesoActual.ETA --;
			tiempoTranscurrido++;


			if (procesoActual.ETA <= 0)
			{
				siguienteProceso();
			}

			actualizaGridView();

			//dgvProcesos.Items.Refresh();
		}

		private void siguienteProceso()
		{
			var indice = loteActual.getListaProcesos().IndexOf(procesoActual);
			if (indice < 4 && indice + 1 < loteActual.getListaProcesos().Count)
			{
				procesoActual.Termino = true;
				procesoActual.resolverEcuacion();
				procesoActual = loteActual.getListaProcesos()[loteActual.getListaProcesos().IndexOf(procesoActual) + 1]; // Cambia de proceso
				tiempoTranscurrido = 0;
			}
			else
			{
				procesoActual.Termino = true;
				procesoActual.resolverEcuacion();
				loteActual.Termino = true;

				if (listaLotes.IndexOf(loteActual) + 1 < listaLotes.Count)
				{
					loteActual = listaLotes[listaLotes.IndexOf(loteActual) + 1];
					procesoActual = loteActual.getListaProcesos()[0];
					tiempoTranscurrido = 0;
					//dgvLotes.SelectedItem = loteActual;

				}
				else
				{
					dt.Stop();
					cambiarEstadoDataGrid();
                    actualizaGridView();
					txbEstado.Text = "Terminado";
				}
			}
		}

		private void btnIniciarCronometro_Click(object sender, RoutedEventArgs e)
		{
			btnIniciarCronometro.IsEnabled = false;

			loteActual = listaLotes[0];
			procesoActual = loteActual.getListaProcesos()[0];

			cambiarEstadoDataGrid();

			dt.Start();

			this.Focus();
            estaCorriendo = true;
			txbEstado.Text = "Corriendo";
		}

		private void actualizaGridView()
		{
			dgvLotes.ItemsSource = null;
			dgvProcesos.ItemsSource = null;

			dgvLotes.ItemsSource = listaLotes;
			dgvProcesos.ItemsSource = loteActual.getListaProcesos();
		}

		private void btnCrearProceso_Click(object sender, RoutedEventArgs e)
		{
			int numeroProcesos = int.Parse(cmbNumeroProcesos.SelectedItem.ToString());

			for (int i = 0; i < numeroProcesos; i++)
			{
				crearProceso
					(
						"Astudillo",
						cmbOperacion.Items[rnd.Next(0,cmbOperacion.Items.Count - 1 )].ToString(),
						rnd.Next(100).ToString(),
						rnd.Next(1,100).ToString(),
						rnd.Next(1,15)
					);
			}

			btnCrearProceso.IsEnabled = false;
		}

		private void crearProceso(string programador, string operacion, string operando1, string operando2, int tiempo)
		{
			Proceso nuevoProceso = new Proceso
					(
						programador,
						operacion,
						operando1,
						operando2,
						tiempo
					);

			//listaId.Add(id);

			globalMaximo += nuevoProceso.ETA;
			if (loteActual.getListaProcesos().Count >= MAX_PROCESOS_LOTE)
			{
				Lote nuevoLote = new Lote();
				nuevoLote.setProceso(nuevoProceso);
				listaLotes.Add(nuevoLote);
				loteActual = nuevoLote;
			}
			else
			{
				loteActual.setProceso(nuevoProceso);
			}

			actualizaGridView();

			txbMaximoGlobal.Text =	(globalMaximo / 60).ToString().PadLeft(2, '0') + ":" +
									(globalMaximo % 60).ToString().PadLeft(2, '0');
		}

		private void dgvLotes_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			dgvProcesos.ItemsSource = ((Lote)dgvLotes.SelectedItem).getListaProcesos();
		}

		private void cambiarEstadoDataGrid()
		{
			dgvLotes.IsEnabled = !dgvLotes.IsEnabled;
			dgvProcesos.IsEnabled = !dgvProcesos.IsEnabled;
		}

        private void mandarAlUltimo()
        {
            var indiceUltimo = loteActual.getListaProcesos().Count - 1;

            Proceso tmp = loteActual.getListaProcesos()[indiceUltimo];
            loteActual.getListaProcesos()[indiceUltimo] = procesoActual;

            var indiceActual = loteActual.getListaProcesos().IndexOf(procesoActual);

            loteActual.getListaProcesos()[indiceActual] = tmp;

            procesoActual = tmp;
        }    

		private void cambiarEstadoGUI()
		{
			
		}

		private void Grid_KeyDown(object sender, KeyEventArgs e)
		{
			//MessageBox.Show(e.Key.ToString());
		}

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            var procesoAnterior = procesoActual;

            if (estaCorriendo && e.Key == Key.W)
            {
                siguienteProceso();
                procesoAnterior.Resultado = "Error";
            }
            else if (estaCorriendo && e.Key == Key.P)
            {
                dt.Stop();
                estaPausado = true;
				txbEstado.Text = "Pausado";
                //cambiarEstadoDataGrid();
            }
            else if ( estaPausado && e.Key == Key.C )
            {
                dt.Start();
                estaPausado = false;
				txbEstado.Text = "Corriendo";
                //cambiarEstadoDataGrid();
            }

            else if ( estaCorriendo && e.Key == Key.E)
            {
                mandarAlUltimo();
            }
        }
    }

    class Proceso
	{
		public static int idProceso = 1;

		public int Id { get; set; }
		public String Programador { get; set; }
		public String Operador1 { get; set; }
		public String Operacion { get; set; }
		public String Operador2 { get; set; }
		public string Resultado { get; set; }
		public int ETA { get; set; }
		public bool Termino { get; set; }

		public Proceso(String Programador, String operacion, String operador1, String operador2, int ETA)
		{
			this.Id = idProceso;
			//this.Id = id;
			this.Programador = Programador;
			this.Operacion = operacion;
			this.Operador1 = operador1;
			this.Operador2 = operador2;
			this.ETA = ETA;
			this.Termino = false;
			idProceso++;
		}

		public void resolverEcuacion()
		{
			int op1 = int.Parse(Operador1),
				op2 = int.Parse(Operador2);

			switch (Operacion)
			{
				case "+":
					Resultado = (op1 + op2).ToString();
					break;
				case "-":
					Resultado = (op1 - op2).ToString();
					break;
				case "*":
					Resultado = (op1 * op2).ToString();
					break;
				case "/":
					Resultado = (op1 / op2).ToString();
					break;
				case "%":
					Resultado = (op1 % op2).ToString();
					break;
				case "^":
					Resultado = (op1 ^ op2).ToString();
					break;
				case "%%":
					Resultado = ((op1 / 100) * op2).ToString();
					break;
				default:
					Resultado = (-1).ToString();
					break;
			}
		}
	}

	class Lote
	{
		public static int idLote = 1;

		public int Id { get; set; }
		public int ETA { get; set; }
		public bool Termino { get; set; }

		private List<Proceso> listaProcesos;

		public Lote()
		{
			this.listaProcesos = new List<Proceso>();
			this.Id = idLote;
			this.ETA = 0;

			idLote++;
		}

		public void setProceso(Proceso proceso)
		{
			ETA = ETA + proceso.ETA;
			listaProcesos.Add(proceso);
		}

		public List<Proceso> getListaProcesos() { return this.listaProcesos; }
	}
}