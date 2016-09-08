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
		public static int idProceso = 0;
		public static int segundos = 0;

		private BindingList<Lote> listaLotes;
		private BindingList<Proceso> listaProceso;

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
			listaProceso = new BindingList<Proceso>();

			dgvProcesos.ItemsSource = listaProceso;
		}

		private void dispatcherTimer_Tick(object sender, EventArgs e)
		{
			segundos++;

			String tiempo =	(segundos / 60).ToString().PadLeft(2, '0') + ":" 
						  + (segundos % 60).ToString().PadLeft(2,'0');

			txbCronometro.Text = tiempo;

			((DataGridRow)dgvProcesos.Items[0]).Background = new SolidColorBrush(Colors.Green);
			//dgvProcesos.Items.Refresh();
		}

		private void btnIniciarCronometro_Click(object sender, RoutedEventArgs e)
		{
			btnIniciarCronometro.IsEnabled = false;

			dt.Start();
		}

		private void btnCrearProceso_Click(object sender, RoutedEventArgs e)
		{
			if ( txbProgramador.Text != "" )
			{
				Proceso nuevoProceso = new Proceso
					(
						txbProgramador.Text,
						cmbOperacion.SelectedItem.ToString(),
						cmbOperando1.SelectedItem.ToString(),
						cmbOperando2.SelectedItem.ToString(),
						(int) cmbTiempoEstimado.SelectedItem
					);

				listaProceso.Add(nuevoProceso);
			}
		}
	}

	class Proceso
	{
		public int id;
		public String Programador { get; set; }
		public String Operador1 { get; set; }
		public String Operacion { get; set; }
		public String Operador2 { get; set; }
		public float Resultado { get; set; }
		public int TiempoEstimado { get; set; }

		public Proceso(String Programador, String operacion, String operador1, String operador2, int tiempoEstimado)
		{
			this.id = MainWindow.idProceso;
			this.Programador = Programador;
			this.Operacion = operacion;
			this.Operador1 = operador1;
			this.Operador2 = operador2;
			this.TiempoEstimado = tiempoEstimado;
			MainWindow.idProceso++;
		}
	}

	class Lote
	{
		public List<Proceso> listaProcesos;

		public Lote()
		{
			this.listaProcesos = new List<Proceso>();
		}

		public void setProceso(Proceso proceso)
		{
			listaProcesos.Add(proceso);
		}
	}
}