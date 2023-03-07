using SyncToy;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

namespace SyncToyAid.Logic
{
	internal class SyncToyPair
	{
		#region Private Members

		private readonly BinaryFormatter formatter;
		private readonly List<SyncEngineConfig> configList;

		#endregion

		#region Constructor(s)

		/// <summary>
		/// Initializes a new instance of the <see cref="SyncToyPair"/> class.
		/// </summary>
		public SyncToyPair()
		{
			formatter = new BinaryFormatter();

			formatter.AssemblyFormat = FormatterAssemblyStyle.Simple;

			configList = new List<SyncEngineConfig>();
		}

		#endregion

		#region Public Members

		/// <summary>
		/// Gets the list.
		/// </summary>
		/// <value>The list.</value>
		public List<SyncEngineConfig> List
		{
			[DebuggerNonUserCode]
			get { return configList; }
		} 

		#endregion

		#region Public Methods

        /// <summary>
        /// Reads this instance.
        /// </summary>
        /// <param name="path">The path.</param>
        public void Read(string path)
		{
			configList.Clear();

            using (StreamReader reader = new StreamReader(path))
			{
				if (reader.BaseStream.Length > 0)
				{
					do
					{
						SyncEngineConfig config = (SyncEngineConfig)formatter.Deserialize(reader.BaseStream);

						configList.Add(config);
					}
					while (reader.BaseStream.Position < reader.BaseStream.Length);
				}

				reader.Close();
			}
		}

        /// <summary>
        /// Writes this instance.
        /// </summary>
        /// <param name="path">The path.</param>
		public void Write(string path)
		{
			using (StreamWriter writer = new StreamWriter(path))
			{
				foreach (SyncEngineConfig config in configList)
				{
					formatter.Serialize(writer.BaseStream, config);

					writer.Flush();
				}

				writer.Close();
			}
		}

		#endregion
	}
}
