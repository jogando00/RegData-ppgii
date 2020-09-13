using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace v1
{
    class Program
    {
        static void Main(string[] args)
        {
            bool w = true;
            CSVFile csv = new CSVFile();
            csv.create(args[0]);

            while (w)
            {
                Console.WriteLine("\n==========================\n" +
                    "========== MENU ==========\n" +
                    "==========================\n\n " +
                    "[1] Agregar\n " +
                    "[2] Listar\n " +
                    "[3] Buscar\n " +
                    "[4] Editar\n " +
                    "[5] Eliminar\n " +
		    "[S] Salir\n");

                string opt1 = Console.ReadLine();

                bool through = true;
                switch (opt1.ToUpper())
                {
                    case "1":
                        csv.add_line();
                        break;
                    case "2":
                        csv.list();
                        break;
                    case "3":
                        List<string> reg = csv.search("buscar");
                        Console.WriteLine(reg[0]);
                        break;
                    case "4":
                        csv.edit();
                        break;
                    case "5":
                        csv.remove();
                        break;
		    case "S":
			w = false;
			through = false;
			break;
                    default:
                        Console.WriteLine("Valor incorrecto, por for ingresar una opcion numerica del 1 al 5");
                        through = false;
                        break;
                }

                if (through)
                {
                    Console.WriteLine("\n [S] Salir \n [Other key] Menu \n");
                    string opt2 = Console.ReadLine();

                    if (opt2.ToUpper() == "S") w = false;
                }
            }
        }
    }

    class CSVFile
    {
        public bool exists;
        public string file_path;

        public void check(string path)
        {
            exists = File.Exists(path);
        }

        public void create(string path)
        {
            check(path);
            if (!exists)
            {
                try
                {
                    var f = new FileStream(path, FileMode.Create);
                    f.Dispose();

                    using (StreamWriter stw = new StreamWriter(path))
                    {
                        stw.WriteLine("Cedula,Nombre,Apellido,Edad");
                        stw.Close();
                    }

                }catch(Exception err)
                {
                    Console.WriteLine(err.Message);
                }
            }
            file_path = path;
        }

        public void add_line()
        {
            string[] val = { "cedula", "nombre", "apellido", "edad" };
            string[] data = new string[4];

            for(int i = 0; i < val.Length; i++) {
                Console.WriteLine("Ingrese {0}:", val[i]);
                data[i] = Console.ReadLine();
            }

            string a = string.Join(",", data);
            using (StreamWriter stw = new StreamWriter(file_path, true))
            {
                stw.WriteLine(a);
                stw.Close();
            }
        }

        public void list()
        {
            string[] file = File.ReadAllLines(file_path);
            foreach(string f in file)
            {
                Console.WriteLine(f);
            }
        }

        public List<string> search(string opt)
        {
            Console.WriteLine("Ingrese la cedula a {0}:", opt);
            string cedula = Console.ReadLine();

            List<string> rt = new List<string>();
            bool found = false;
            string[] file = File.ReadAllLines(file_path);
            for(int i = 0; i < file.Length; i++)
            {
                string[] reg = file[i].Split(",");
                if(reg[0] == cedula)
                {
                    found = true;
                    rt.Add(file[i]);
                    rt.Add(i.ToString());
                }
            }
            if (!found)
            {
                rt.Add("Registro no encontrado");
                rt.Add("0");
            }

            return rt;
        }

        public void edit()
        {
            List<string> reg = search("editar");

            if(reg[1] != "0")
            {
                string[] file = File.ReadAllLines(file_path);

                string[] prev = reg[0].Split(",");
                string[] val = { "cedula", "nombre", "apellido", "edad" };
                string[] data = new string[4];

                data[0] = prev[0];
                for (int i = 1; i < val.Length; i++)
                {
                    Console.WriteLine("Ingrese {0}:     Valor anterior: {1}", val[i], prev[i]);
                    data[i] = Console.ReadLine();
                }
                string a = string.Join(",", data);

                file[Convert.ToInt32(reg[1])] = a;
                File.WriteAllLines(file_path, file);

                Console.WriteLine("\n Registro editado.");
            }
            else
            {
                Console.WriteLine(reg[0]);
            }

        }

        public void remove()
        {
            List<string> reg = search("eliminar");

            if (reg[1] != "0")
            {
                string[] file = File.ReadAllLines(file_path);
                List<string> l_file = new List<string>(file);
                l_file.RemoveAt(Convert.ToInt32(reg[1]));

                var r_file = l_file.ToArray();

                File.WriteAllLines(file_path, r_file);
		
		Console.WriteLine("\n Registro eliminado.");
            }
        }
    }
}

