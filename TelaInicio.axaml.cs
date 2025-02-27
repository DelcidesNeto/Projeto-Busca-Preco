using System;
using System.IO;
using System.Reflection;

//using System.Reflection;
//using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using FirebirdSql.Data.FirebirdClient;
using Tmds.DBus.Protocol;
//using Avalonia.Interactivity;


namespace SoftwareBuscaPreco;


public class SelectLanguageOfConsult: TelaInicio{
    public class Firebird{
        private string GetPathAtual(){
            string ExePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\\db\\NSC-Amanhecer.FDB";
            //string FilePath = Path.Combine(ExePath, "db", "NSC-Amanhecer.FDB");
            return ExePath;
        }
        public Produto BuscarProduto(string ACodigoDeBarrasProduto){
            //string CaminhoBaseDeDados = $"User=SYSDBA;Password=masterkey;Database={GetPathAtual()};DataSource=localhost;Dialect=2;";
            string CaminhoBaseDeDados = "User=SYSDBA;Password=masterkey;Database=C:\\NETSide\\db\\NSC nutri paiva.FDB;DataSource=192.168.50.249;Dialect=2;";
            FbConnection Banco = new FbConnection(CaminhoBaseDeDados);
            //Produto DadosProduto;
            try{
                Banco.Open();
                string CmdQuery = "SELECT NOMPRO, PVENDA FROM PRODUTOS WHERE CODPRO = @CODIGO_DE_BARRAS";
                FbCommand Query = new FbCommand(CmdQuery, Banco);
                Query.Parameters.AddWithValue("@CODIGO_DE_BARRAS", ACodigoDeBarrasProduto);
                FbDataReader ResultadoQuery = Query.ExecuteReader();
                Produto DadosProduto = new Produto("Produto não encontrado!", "");    
                while (ResultadoQuery.Read()){
                    DadosProduto = new Produto(ResultadoQuery["NOMPRO"].ToString(), $"R${Convert.ToDecimal(ResultadoQuery["PVENDA"]).ToString("N2")}");    
                }
                return DadosProduto;
                
                
            }
            finally{
                Console.WriteLine("Tudo Certo");
            }
        }
    }
}



public partial class TelaInicio : Window{  
    
    public record Produto(string NomeProduto, string PrecoProduto);
    public TelaInicio()
    {
        InitializeComponent();
        //this.Loaded += FocarBarcode;
        //this.LayoutUpdated += FocarBarcode;
    }
    private void ModifyBarcodeInput(object Sender, KeyEventArgs Teclado){
        
        if (!Barcode.IsFocused){
            if(Teclado.Key >= Key.D0 && Teclado.Key <= Key.D9 || Teclado.Key >= Key.NumPad0 && Teclado.Key <= Key.NumPad9){
                Barcode.Focus();
            }
        }
    }
    private void OnlyNumbers(object Sender, KeyEventArgs Teclado){
        Console.WriteLine(Teclado.KeySymbol);
        if(Teclado.Key >= Key.D0 && Teclado.Key <= Key.D9 || Teclado.Key >= Key.NumPad0 && Teclado.Key <= Key.NumPad9){}
        else if (Teclado.Key == Key.Enter){
            Console.WriteLine($"Buscando pelo produto: {Barcode.Text}...");
            FocusManager.ClearFocus();
            Produto DadosProduto = new SelectLanguageOfConsult.Firebird().BuscarProduto(Barcode.Text);
            NomeProduto.Text = DadosProduto.NomeProduto;
            PrecoProduto.Text = DadosProduto.PrecoProduto;
            
        }
        else{
            Teclado.Handled = true;
        }
    // else if (Teclado.Key == Key.Back){
    //         if (Barcode.Text.Length > 0){
    //             Barcode.Text = Barcode.Text.Substring(0, Barcode.Text.Length-1);
    //         }   
    //     }
    }
}