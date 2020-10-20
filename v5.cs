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

            if(args.Length == 0)
            {
                Console.WriteLine("No args passed, path needed.");
                return;
            }

            CSVFile csv = new CSVFile(args[0]);

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
				    csv.save_file();
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

                    if (opt2.ToUpper() == "S") {
			csv.save_file();
			w = false;
			
		    }
                }
            }
        }
    }

    class Person 
    {
	public string Id {get;}
	public string Name {get;}
	public string LastName {get;}
	private int _packedData;
	public int Age => _packedData >> 4;
	public string Sex => (_packedData & 8) == 8 ? "Masculino" : "Femenino";
	public string State => (_packedData & 4) == 4 ? "Casado" : "Soltero";
	public string Grade => (_packedData & 1) == 1 ? "Media" : (_packedData & 2) == 2 ? "Grado" : (_packedData & 3) == 3 ? "Postgrado" : "Inicial";
	public float Savings {get;}
	public string Password {get;}

	public Person(in string id, in string name, in string lastname, in int age, in string sex, in string state, in string grade, in float savings, in string password)
	{
	    Id = id;
	    Name = name;
	    LastName = lastname;
	    Savings = savings;
	    Password = password;
	    int a = age << 4;
	    if (sex == "M") a = a | 8;
            if (state == "C") a = a | 4;

            switch(grade)
            {
                case "M":
                     a = a | 1;
                     break;
                case "G":
                     a = a | 2;
                     break;
                case "P":
                     a = a | 3;
                     break;
            }

	    _packedData = a;
	} 
	
	internal static Person personFroCsvLine(in string line)
	{	
	    string[] tokens = line.Split(",");
	    (string id, string name, string lastname, float savings, string password, int age) = (tokens[0], tokens[1], tokens[2], float.Parse(tokens[3]), tokens[4], (int.Parse(tokens[5]) >> 4));
	    
	    string sex = (int.Parse(tokens[5]) & 8) == 8 ? "M" : "F";
	    string state = (int.Parse(tokens[5]) & 4) == 4 ? "C" : "S";
	    string grade = (int.Parse(tokens[5]) & 1) == 1 ? "M" : (int.Parse(tokens[5]) & 2) == 2 ? "G" : (int.Parse(tokens[5]) & 3) == 3 ? "P" : "I";
	
	    return new Person(id, name, lastname, age, sex, state, grade, savings, password);	
	}

	public override bool Equals(object obj)
	{	
	    if(obj is Person other){
	    	return Id.Equals(other.Id);
	    }
	    return false;
	}

	public override string ToString()
	{	
	    return $"{Id},{Name},{LastName},{Age},{Sex},{State},{Grade},{Savings},{Password}";
	}
	
	public string forFile() 
	{
	    return $"{Id},{Name},{LastName},{Savings},{Password},{_packedData}";	
	}
    }

    class CSVFile
    {
        public bool exists;
        public string file_path;
	public List<Person> people = new List<Person>();

        public CSVFile(string path)
        {
            try
            {
                exists = File.Exists(path);
                file_path = path;

                if (!exists)
                {
                    try
                    {
                        var f = new FileStream(path, FileMode.Create);
                        f.Dispose();

                        using (StreamWriter stw = new StreamWriter(path))
                        {
                            stw.WriteLine("Cedula,Nombre,Apellido,Ahorros,Password,Data");
                            stw.Close();
                        }

                    }
                    catch (Exception err)
                    {
                        Console.WriteLine(err.Message);
                    }
                }
		
		string[] file = File.ReadAllLines(file_path);
		foreach(string line in file)
		{
		    Person person = Person.personFroCsvLine(line);
		    people.Add(person);
		}
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

	public void save_file()
	{
	     List<string> l_file = new List<string>();
	     foreach(Person person in people)
	     {
		l_file.Add(person.forFile());
	     }
	     
	     string[] file = l_file.ToArray();

	     File.WriteAllLines(file_path, file);
	}

        public string capture(string opt)
        {
            switch (opt)
            {
                case "cedula":
                    return readID();
                case "edad":
                    string number;
                    while (true)
                    {
                        number = readNumber();
                        if (Convert.ToInt32(number) > 120) Console.WriteLine("\nEdad invalida, introduzca otra vez");
                        else break;
                    }
                    return number;
                case "ahorro":
                    return readDecimal();
                case "password":
                    bool equal = false;
                    string pass1, pass2 = ""; 
                    while (!equal)
                    {
                        pass1 = readPassword();
                        Console.WriteLine("\nConfirme password: ");
                        pass2 = readPassword();

                        if (pass1 == pass2) equal = true;
                        else Console.WriteLine("\nPasswords no son iguales, introduzca otra vez");
                    }
                    return pass2;
		case "sexo (M/F)":
		    return readGiven("MF");
		case "estado civil (S/C)":
		    return readGiven("SC");
		case "grado(I/M/G/P)":
		    return readGiven("IMGP");
                default:
                    return Console.ReadLine();
            }
        }

        public void add_line()
        {
            string[] val = { "cedula", "nombre", "apellido", "edad", "sexo (M/F)", "estado civil (S/C)", "grado(I/M/G/P)", "ahorro", "password"  };
            string[] data = new string[9];
            bool cont = true, w = true;

            while(w){

                if (cont)
                {
                    for(int i = 0; i < val.Length; i++) {
                        Console.WriteLine("\nIngrese {0}:", val[i]);
                        data[i] = capture(val[i]);
                    }
                }

                Console.WriteLine("\n [C/G/S] Continuar/Guardar/Salir");
                string opt = Console.ReadLine();

                switch (opt.ToUpper())
                {
                    case "C":
                        cont = true;
                        break;
                    case "G":
                        Person person = new Person(data[0],data[1],data[2],int.Parse(data[3]),data[4],data[5],data[6],float.Parse(data[7]),data[8]);
                        if(!people.Contains(person)){
				people.Add(person);
				Console.WriteLine(" Registro guardado.");
				w = false;
			}
                        else{
				Console.WriteLine("Persona ya existe.");
			}
                        break;
                    case "S":
                        w = false;
                        break;
                    default:
                        cont = false;
                        Console.WriteLine("Opcion invalida");
                        break;

                }
            }



        }

        public void list()
        {
	     Console.WriteLine("Cedula, Nombre, Apellido, Edad, Sexo, Estado Civil, Grado, Ahorro, Password");
	     foreach(Person person in people)
	     {
	     	Console.WriteLine($"{person}");
	     }
        }

        public List<string> search(string opt)
        {
            Console.WriteLine("Ingrese la cedula a {0}:", opt);
            string id = Console.ReadLine();

            List<string> rt = new List<string>();
            bool found = false;
            
            for(int i = 0; i < people.Count; i++)
            {
                if(people[i].Id == id)
                {
                    found = true;
                    rt.Add(people[i].ToString());
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

                string[] prev = reg[0].Split(",");
                string[] val = { "cedula", "nombre", "apellido", "edad", "sexo (M/F)", "estado civil (S/C)", "grado(I/M/G/P)", "ahorro", "password"  };
                string[] data = new string[9];

                data[0] = prev[0];
                for (int i = 1; i < val.Length; i++)
                {           
                    Console.WriteLine("Ingrese {0}:     Valor anterior: {1}", val[i], prev[i]);
                    data[i] = capture(val[i]);
                }
                
		Person person = new Person(data[0],data[1],data[2],int.Parse(data[3]),data[4],data[5],data[6],float.Parse(data[7]),data[8]);

                people[Convert.ToInt32(reg[1])] = person;

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
              
                people.RemoveAt(Convert.ToInt32(reg[1]));
		
		Console.WriteLine("\n Registro eliminado.");
            }
            else
            {
                Console.WriteLine(reg[0]);
            }
        }

        public string readDecimal()
        {
            List<string> res = new List<string>();
            int afterDot = 0;

            while (true)
            {
                ConsoleKeyInfo a = Console.ReadKey(true);
                if (a.KeyChar == (char)10) break;

                bool hasDot()
                {
                    foreach (string r in res)
                    {
                        if (r == ".") return true;
                    }
                    return false;
                }

                if(a.KeyChar == '.')
                {
                    if (!hasDot())
                    {
                        Console.Write(a.KeyChar.ToString());
                        res.Add(a.KeyChar.ToString());
                    }
                }

                foreach (char i in "0123456789")
                {
                    if (a.KeyChar == i && afterDot < 2)
                    {
                        Console.Write(a.KeyChar.ToString());
                        res.Add(a.KeyChar.ToString());
                        if (hasDot()) afterDot++;
                    }
                }

                if(a.Key == ConsoleKey.Backspace)
                {
                    Console.Write("\b");
                    if(res.Count > 0) res.RemoveAt(res.Count - 1);
                    if (hasDot()) afterDot--;
                }
            }
            if (res.Count > 0) return string.Join("", res);
            else
            {
                Console.WriteLine("\n0 valores, introduzca otra vez");
                return readDecimal();
            }
        }

        public string readNumber()
        {
            List<string> res = new List<string>();

            while (true)
            {
                ConsoleKeyInfo a = Console.ReadKey(true);
                if (a.KeyChar == (char)10) break;

                foreach (char i in "0123456789")
                {
                    if (a.KeyChar == i)
                    {
                        Console.Write(a.KeyChar.ToString());
                        res.Add(a.KeyChar.ToString());
                    }
                }

                if (a.Key == ConsoleKey.Backspace)
                {
                    Console.Write("\b");
                    if (res.Count > 0) res.RemoveAt(res.Count - 1);
                }
            }
            if (res.Count > 0) return string.Join("", res);
            else
            {
                Console.WriteLine("\n0 valores, introduzca otra vez");
                return readNumber();
            }
        }

        public string readPassword()
        {
            List<string> res = new List<string>();

            while (true)
            {
                ConsoleKeyInfo a = Console.ReadKey(true);
                if (a.KeyChar == (char)10) break;

                if (a.Key == ConsoleKey.Backspace)
                {
                    Console.Write("\b");
                    if (res.Count > 0) res.RemoveAt(res.Count - 1);
                }
                else
                {
                    Console.Write('*');
                    res.Add(a.KeyChar.ToString());
                }
            }

            if (res.Count > 0) return string.Join("", res);
            else
            {
                Console.WriteLine("\n0 valores, introduzca otra vez");
                return readPassword();
            }
        }

        public string readID()
        {
            List<string> res = new List<string>();
            int max = 0;

            while (max < 11)
            {
                ConsoleKeyInfo a = Console.ReadKey(true);
                if (a.KeyChar == (char)10) break;

                foreach (char i in "0123456789")
                {
                    if (a.KeyChar == i)
                    {
                        Console.Write(a.KeyChar.ToString());
                        res.Add(a.KeyChar.ToString());
                        max++;
                    }
                }

                if (a.Key == ConsoleKey.Backspace)
                {
                    Console.Write("\b");
                    if (res.Count > 0) res.RemoveAt(res.Count - 1);
                    max--;
                }
            }

            if (res.Count > 0) return string.Join("", res);
            else
            {
                Console.WriteLine("\n0 valores, introduzca otra vez");
                return readID();
            }
        }

	public string readGiven(string accepted)
        {
            List<string> res = new List<string>();

            while (true)
            {
                ConsoleKeyInfo a = Console.ReadKey(true);
                if (a.KeyChar == (char)10) break;

                foreach (char i in accepted)
                {
                    if (a.KeyChar == i)
                    {
                        Console.Write(a.KeyChar.ToString());
                        res.Add(a.KeyChar.ToString());
                    }
                }

                if (a.Key == ConsoleKey.Backspace)
                {
                    Console.Write("\b");
                    if (res.Count > 0) res.RemoveAt(res.Count - 1);
                }
            }
            if (res.Count > 0) return string.Join("", res);
            else
            {
                Console.WriteLine("\n0 valores, introduzca otra vez");
                return readGiven(accepted);
            }
        }

    }
}

