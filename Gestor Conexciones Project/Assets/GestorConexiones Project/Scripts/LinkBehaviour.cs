//                                  ┌∩┐(◣_◢)┌∩┐
//																				\\
// LinkBehaviour.cs (07/07/2018)												\\
// Autor: Antonio Mateo (.\Moon Antonio) 	antoniomt.moon@gmail.com			\\
// Descripcion:		Componente central del gestor de conexiones.				\\
// Fecha Mod:		07/07/2018													\\
// Ultima Mod:		Version Inicial												\\
//******************************************************************************\\

#region Librerias
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Net;
#endregion

namespace MoonAntonio
{
	/// <summary>
	/// <para>Componente central del gestor de conexiones.</para>
	/// </summary>
	public class LinkBehaviour : MonoBehaviour
	{
		#region Variables Publicas
		public Slider progressBar;
		public bool UsarNombreOriginal = true;
		public string newNombreArchivo = "x.zip";
		public bool onStart = false;
		public string downloadUrl = string.Empty;
		public string savePath = string.Empty;
		public GameObject btnDescargaFinalizada;
		public GameObject btnIniciarDescarga;
		public bool dataPathPersistente = false;
		public bool isMostrarBytes = true;
		public Text progressText;
		public Text bytesText;
		#endregion

		#region Variables Privadas
		private string uri;
		private bool isCancelado;
		private bool isDescargando;
		private bool isTerminado;
		private WebClient client;
		private float progreso;
		private string bytes;
		#endregion

		#region Inicializadores
		private void Start()
		{
			// Establecer el valor maximo de la barra de progreso(Slider de UI) en 100(%)
			if (progressBar.maxValue != 100f) progressBar.maxValue = 100f;

			// Inicializar a 0
			progressBar.value = 0;

			// Asignar Uri
			uri = downloadUrl;

			// Usar el nombre original del archivo descargado
			if (UsarNombreOriginal) newNombreArchivo = Path.GetFileName(uri);

			// Comprobar si existe el directorio
			DirectoryInfo df = new DirectoryInfo(savePath);
			if (!df.Exists) Directory.CreateDirectory(savePath);


			if (onStart) DescargarArchivo();
		}
		#endregion

		#region Actualizadores
		private void Update()
		{
			if (isDescargando)
			{
				progressBar.value = progreso;
				progressText.text = progreso.ToString() + "% ";

				if (isMostrarBytes) bytesText.text = "Recibido : " + bytes + " kb";
			}


			if (isTerminado)
			{
				if (isCancelado)
				{
					bytesText.text = "Cancelado";
					progressText.text = "0 %";
					isTerminado = false;
				}
				else
				{
					if (!btnDescargaFinalizada.activeSelf)
					{
						btnDescargaFinalizada.SetActive(true);
						btnIniciarDescarga.SetActive(false);
					}
					bytesText.text = "Recibido : " + "Completado";
					progressText.text = "100 %";
				}
			}

		}
		#endregion

		#region API
		public void DescargarArchivo()
		{
			// Desactivamos la posibilidad de llamar de nuevo al metodo
			btnIniciarDescarga.SetActive(false);

			// Reseteamos la descarga y inicializamos el objeto
			isCancelado = false;
			client = new WebClient();

			// Comprobamos la persistencia para prevenir problemas futuros
			if (!dataPathPersistente)
				client.DownloadFileAsync(new System.Uri(downloadUrl), savePath + "/" + newNombreArchivo);
			else
				client.DownloadFileAsync(new System.Uri(uri), Application.persistentDataPath + "/" + newNombreArchivo);

			// Iniciamos la descarga y nos subscribimos a los eventos de WebClient()
			isDescargando = true;
			client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DescargaProgreso);
			client.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler(DescargaCompletada);
		}

		public void CargarNivel(string name)
		{
			UnityEngine.SceneManagement.SceneManager.LoadScene(name);
		}

		public void CancelarDescarga()
		{
			isCancelado = true;
			if (client != null)
			{
				client.CancelAsync();
			}

			btnIniciarDescarga.SetActive(true);
		}
		#endregion

		#region Eventos
		protected void DescargaCompletada(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
		{
			if (isCancelado)
			{
				Debug.Log("Cancelado.");
				isDescargando = false;
				isTerminado = true;
			}
			else
			{
				if (e.Error == null)
				{
					Debug.Log("Completado");
					isDescargando = false;
					isTerminado = true;
				}
				else
					Debug.Log(e.Error.ToString());
			}
		}

		protected void DescargaProgreso(object sender, DownloadProgressChangedEventArgs e)
		{
			progreso = (e.BytesReceived * 100 / e.TotalBytesToReceive);
			bytes = e.BytesReceived / 1000 + " / " + e.TotalBytesToReceive / 1000;
		}
		#endregion

		#region Metodos Privados
		private void OnDisable()
		{
			isCancelado = true;
			if (client != null) client.CancelAsync();
		}
		#endregion
	}
}