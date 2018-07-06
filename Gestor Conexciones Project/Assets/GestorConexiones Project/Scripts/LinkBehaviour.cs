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
		#endregion

		#region Variables Privadas
		private string uri;
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
	}
}