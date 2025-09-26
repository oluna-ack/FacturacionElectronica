using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data;
using System.Data.Common;

namespace firmaEnvioSri.App_Code.DAL
{
    public class clsNeg_AccesoDatos
    {
        #region CONSTRUCTOR
        Database objDataBase = null;
        public clsNeg_AccesoDatos(string pCandenaConexion)
        {


          
            try
            {
                objDataBase = DatabaseFactory.CreateDatabase(pCandenaConexion);
            }
            catch (Exception ex)
            {
                // Mostrar error en consola o log
                Console.WriteLine("Error al crear la base de datos con Enterprise Library.");
                Console.WriteLine("Nombre de la cadena: " + pCandenaConexion);
                Console.WriteLine("Mensaje: " + ex.Message);
                Console.WriteLine("InnerException: " + ex.InnerException?.Message);
                Console.WriteLine("StackTrace: " + ex.StackTrace);

                // Puedes volver a lanzar la excepción si quieres que falle completamente
                // throw;

                // O manejar de forma personalizada (ej. dejar objDataBase como null)
            }

        }
        #endregion

        #region CONSULTAS
        public DataSet TraerDataSet(string pProcedimientoAlmacenado)
        {
            return objDataBase.ExecuteDataSet(objDataBase.GetStoredProcCommand(pProcedimientoAlmacenado));
        }
        public DataSet TraerDataSet(string pProcedimientoAlmacenado, params object[] pParams)
        {
            DbCommand objCommand = objDataBase.GetStoredProcCommand(pProcedimientoAlmacenado, pParams);
            objCommand.CommandTimeout = 0;  //Si no se pone da 40s, con 0 espera por siempre
            DataSet objDataSet = new DataSet();
            objDataSet = objDataBase.ExecuteDataSet(objCommand);
            return objDataSet;
        }
        public DataTable TraerDataTable(string pProcedimientoAlmacenado)
        {
            DbCommand objCommand = objDataBase.GetStoredProcCommand(pProcedimientoAlmacenado);
            objCommand.CommandTimeout = 0;  //Si no se pone da 40s, con 0 espera por siempre
            DataTable objDataTable = new DataTable();
            objDataTable.Load(objDataBase.ExecuteReader(objCommand));
            return objDataTable;
        }
        public DataTable TraerDataTable(string pProcedimientoAlmacenado, params object[] pParams)
        {
            DbCommand objCommand = objDataBase.GetStoredProcCommand(pProcedimientoAlmacenado, pParams);
            objCommand.CommandTimeout = 0;  //Si no se pone da 40s, con 0 espera por siempre
            DataTable objDataTable = new DataTable();
            objDataTable.Load(objDataBase.ExecuteReader(objCommand));
            return objDataTable;
        }
        public IDataReader TraerDataReader(string pProcedimientoAlmacenado)
        {
            DbCommand objCommand = objDataBase.GetStoredProcCommand(pProcedimientoAlmacenado);
            objCommand.CommandTimeout = 0;  //Si no se pone da 40s, con 0 espera por siempre
            return objDataBase.ExecuteReader(objCommand);
        }
        public IDataReader TraerDataReader(string pProcedimientoAlmacenado, params object[] pParams)
        {
            DbCommand objCommand = objDataBase.GetStoredProcCommand(pProcedimientoAlmacenado, pParams);
            objCommand.CommandTimeout = 0;  //Si no se pone da 40s, con 0 espera por siempre
            return objDataBase.ExecuteReader(objCommand);
        }
        public object TraerValor(string pProcedimientoAlmacenado)
        {
            DbCommand objCommand = objDataBase.GetStoredProcCommand(pProcedimientoAlmacenado);
            objCommand.CommandTimeout = 0;  //Si no se pone da 40s, con 0 espera por siempre
            return objDataBase.ExecuteScalar(objCommand);
        }
        public object TraerValor(string pProcedimientoAlmacenado, params object[] pParams)
        {
            DbCommand objCommand = objDataBase.GetStoredProcCommand(pProcedimientoAlmacenado, pParams);
            objCommand.CommandTimeout = 0;  //Si no se pone da 40s, con 0 espera por siempre
            return objDataBase.ExecuteScalar(objCommand);
        }
        #endregion

        #region EJECUTAR
        public int Ejecutar(string pProcedimientoAlmacenado)
        {
            DbCommand objCommand = objDataBase.GetStoredProcCommand(pProcedimientoAlmacenado);
            objCommand.CommandTimeout = 0;  //Si no se pone da 40s, con 0 espera por siempre
            return objDataBase.ExecuteNonQuery(objCommand);
        }
        public int Ejecutar(string pProcedimientoAlmacenado, params object[] pParams)
        {
            DbCommand objCommand = objDataBase.GetStoredProcCommand(pProcedimientoAlmacenado, pParams);
            objCommand.CommandTimeout = 0;  //Si no se pone da 40s, con 0 espera por siempre
            return objDataBase.ExecuteNonQuery(objCommand);
        }
        #endregion

    }
}
