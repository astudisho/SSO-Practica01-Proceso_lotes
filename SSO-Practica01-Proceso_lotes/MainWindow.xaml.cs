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
		public const int MAX_PROCESOS_LOTE = 5;

		private BindingList<Lote> listaLotes;
		private List<int> listaId;

		private Lote loteActual;
		private Proceso procesoActual;
		private int tiempoMaxEstimado;

		DispatcherTimer dt;

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
				cmbTiempoEstimado.Items.Add(i);
			}

			cmbOperando1.SelectedIndex = 0;
			cmbOperando2.SelectedIndex = 0;
			cmbTiempoEstimado.SelectedIndex = 0;

			dt = new DispatcherTimer();

			dt.Tick += dispatcherTimer_Tick;
			dt.Interval = new System.TimeSpan(0, 0, 1);

			listaLotes = new BindingList<Lote>();
			dgvLotes.ItemsSource = listaLotes;

			listaId = new List<int>();

			loteActual = new Lote();
			listaLotes.Add(loteActual);
			dgvProcesos.ItemsSource = loteActual.getListaProcesos();

			tiempoMaxEstimado = 0;
		}

		private void dispatcherTimer_Tick(object sender, EventArgs e)
		{
			segundos++;

			String tiempo =	(segundos / 60).ToString().PadLeft(2, '0') + ":" 
						  + (segundos % 60).ToString().PadLeft(2,'0');

			txbCronometro.Text = tiempo;

			//dgvProcesos.Items.Refresh();
		}

		private void btnIniciarCronometro_Click(object sender, RoutedEventArgs e)
		{
			btnIniciarCronometro.IsEnabled = false;

			dt.Start();
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
			if (txbProgramador.Text != "" && txbId.Text != "")
			{
				int id = int.Parse(txbId.Text);

				if (listaId.Contains(id))
				{
					MessageBox.Show("El id de proceso ya existe");
					return;
				}

				Proceso nuevoProceso = new Proceso
					(
						int.Parse( txbId.Text ),
						txbProgramador.Text,
						cmbOperacion.SelectedItem.ToString(),
						cmbOperando1.SelectedItem.ToString(),
						cmbOperando2.SelectedItem.ToString(),
						(int)cmbTiempoEstimado.SelectedItem
					);

				listaId.Add(id);

				if (loteActual.getListaProcesos().Count >= MAX_PROCESOS_LOTE)
				{
					Lote nuevoLote = new Lote();
					nuevoLote.setProceso(nuevoProceso);
					listaLotes.Add(nuevoLote);
					loteActual = nuevoLote;
				}
				else
				{
					//MessageBox.Show("Agregado lote actual tiempo " + loteActual.ETA.ToString());
					loteActual.setProceso(nuevoProceso);
				}
				actualizaGridView();
			}
			else
				MessageBox.Show("Escriba un nombre de programador", "Datos faltantes");
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
		public float Resultado { get; set; }
		public int ETA { get; set; }
		public bool Termino { get; set; }

		public Proceso(int Id, String Programador, String operacion, String operador1, String operador2, int ETA)
		{
			//this.Id = idProceso;
			this.Id = Id;
			this.Programador = Programador;
			this.Operacion = operacion;
			this.Operador1 = operador1;
			this.Operador2 = operador2;
			this.ETA = ETA;
			this.Termino = false;
			idProceso++;
		}
	}

	class Lote
	{
		public static int idLote = 1;

		public int Id { get; set; }
		public int ETA { get; set; }
		public bool Termino { get; set; }

		private BindingList<Proceso> listaProcesos;

		public Lote()
		{
			this.listaProcesos = new BindingList<Proceso>();
			this.Id = idLote;
			this.ETA = 0;

			idLote++;
		}

		public void setProceso(Proceso proceso)
		{
			ETA = ETA + proceso.ETA;
			listaProcesos.Add(proceso);
		}

		public BindingList<Proceso> getListaProcesos() { return this.listaProcesos; }
	}
}