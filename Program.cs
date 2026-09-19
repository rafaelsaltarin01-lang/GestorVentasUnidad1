using System;
using System.Collections.Generic;

class Program
{
    // Listas paralelas para almacenar la información en memoria (Sin POO)
    static List<string> nombresProductos = new List<string>();
    static List<decimal> preciosProductos = new List<decimal>();
    static List<int> stockProductos = new List<int>();
    static List<int> unidadesVendidasPorProducto = new List<int>();

    // Variables de control de caja y estadísticas
    static int totalVentasRealizadas = 0;
    static decimal acumuladoCaja = 0m;
    static List<decimal> montosVentas = new List<decimal>();

    static void Main(string[] args)
    {
        int opcion;
        do
        {
            ImprimirEncabezado("SISTEMA GESTOR DE VENTAS E INVENTARIO (MINI-POS)");
            Console.WriteLine("1. Registrar nuevo producto en inventario");
            Console.WriteLine("2. Consultar inventario completo");
            Console.WriteLine("3. Registrar una venta");
            Console.WriteLine("4. Ver reporte de caja y estadísticas diarias");
            Console.WriteLine("5. Salir");
            Console.WriteLine("====================================================");
            
            opcion = LeerEntero("Seleccione una opción (1-5): ", 1, 5);

            switch (opcion)
            {
                case 1:
                    RegistrarProducto();
                    break;
                case 2:
                    ConsultarInventario();
                    break;
                case 3:
                    RegistrarVenta();
                    break;
                case 4:
                    VerReporteCaja();
                    break;
                case 5:
                    ImprimirEncabezado("¡Gracias por usar el Mini-POS! Hasta pronto.");
                    break;
            }

            if (opcion != 5)
            {
                Console.WriteLine("\nPresione cualquier tecla para continuar...");
                Console.ReadKey();
            }

        } while (opcion != 5);
    }

   

    //Lectura Segura de Enteros
    static int LeerEntero(string mensaje, int min, int max)
    {
        int valor;
        while (true)
        {
            Console.Write(mensaje);
            string entrada = Console.ReadLine();
            try
            {
                if (int.TryParse(entrada, out valor))
                {
                    if (valor >= min && valor <= max)
                    {
                        return valor;
                    }
                    Console.WriteLine($"[ERROR] Opción fuera de rango. Ingrese un valor entre {min} y {max}.");
                }
                else
                {
                    Console.WriteLine("[ERROR] Entrada no válida. Debe ingresar un número entero.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Ocurrió un error inesperado: {ex.Message}");
            }
        }
    }

    // Lectura Segura de Decimales
    static decimal LeerDecimal(string mensaje, decimal min)
    {
        decimal valor;
        while (true)
        {
            Console.Write(mensaje);
            string entrada = Console.ReadLine();
            try
            {
                if (decimal.TryParse(entrada, out valor))
                {
                    if (valor > min)
                    {
                        return valor;
                    }
                    Console.WriteLine($"[ERROR] El valor debe ser mayor a {min.ToString("N2")}.");
                }
                else
                {
                    Console.WriteLine("[ERROR] Entrada no válida. Debe ingresar un número decimal.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Ocurrió un error inesperado: {ex.Message}");
            }
        }
    }

    // Lógica de Cálculo de Factura
    static decimal CalcularFactura(decimal precio, int cantidad, bool tieneDescuento, out decimal montoIva, out decimal montoDescuento)
    {
        decimal subtotal = precio * cantidad;
        montoDescuento = tieneDescuento ? subtotal * 0.10m : 0m;
        decimal baseImponible = subtotal - montoDescuento;
        montoIva = baseImponible * 0.19m;
        decimal totalPagar = baseImponible + montoIva;
        return totalPagar;
    }

    // Presentación de Encabezados / Tablas
    static void ImprimirEncabezado(string titulo)
    {
        Console.Clear();
        Console.WriteLine("====================================================");
        Console.WriteLine($"   {titulo}");
        Console.WriteLine("====================================================");
    }

   
    static void RegistrarProducto()
    {
        ImprimirEncabezado("REGISTRAR NUEVO PRODUCTO");
        
        string nombre;
        while (true)
        {
            Console.Write("Ingrese el nombre del producto: ");
            nombre = Console.ReadLine()?.Trim() ?? "";

            if (string.IsNullOrEmpty(nombre))
            {
                Console.WriteLine("[ERROR] El nombre no puede estar vacío.");
                continue;
            }

            // Validamos que no exista (insensible a mayúsculas/minúsculas)
            bool existe = false;
            foreach (string prod in nombresProductos)
            {
                if (prod.Equals(nombre, StringComparison.OrdinalIgnoreCase))
                {
                    existe = true;
                    break;
                }
            }

            if (existe)
            {
                Console.WriteLine("[ERROR] Ya existe un producto registrado con ese nombre.");
            }
            else
            {
                break;
            }
        }

        decimal precio = LeerDecimal("Ingrese el precio unitario ($): ", 0m);
        int stock = LeerEntero("Ingrese el stock inicial (>= 0): ", 0, int.MaxValue);

        // Guardar en listas
        nombresProductos.Add(nombre);
        preciosProductos.Add(precio);
        stockProductos.Add(stock);
        unidadesVendidasPorProducto.Add(0);

        Console.WriteLine($"\n[OK] Producto '{nombre}' registrado exitosamente.");
    }

    static void ConsultarInventario()
    {
        ImprimirEncabezado("CONSULTA DE INVENTARIO COMPLETO");

        if (nombresProductos.Count == 0)
        {
            Console.WriteLine("No hay productos registrados en el inventario actualmente.");
            return;
        }

        for (int i = 0; i < nombresProductos.Count; i++)
        {
            string alerta = stockProductos[i] < 5 ? " [ALERTA: BAJO STOCK]" : "";
            Console.WriteLine($"{i + 1}. {nombresProductos[i],-25} | Precio: $ {preciosProductos[i],10:N2} | Stock: {stockProductos[i],3}{alerta}");
        }
    }

    static void RegistrarVenta()
    {
        ImprimirEncabezado("REGISTRAR VENTA");

        if (nombresProductos.Count == 0)
        {
            Console.WriteLine("No hay productos disponibles para la venta.");
            return;
        }

        // Mostrar listado rápido
        for (int i = 0; i < nombresProductos.Count; i++)
        {
            string alerta = stockProductos[i] < 5 ? " [ALERTA: BAJO STOCK]" : "";
            Console.WriteLine($"{i + 1}. {nombresProductos[i]} | Precio: $ {preciosProductos[i]:N2} | Stock: {stockProductos[i]}{alerta}");
        }
        Console.WriteLine();

        int seleccion = LeerEntero($"Seleccione el número del producto a vender (1-{nombresProductos.Count}): ", 1, nombresProductos.Count);
        int index = seleccion - 1;

        if (stockProductos[index] == 0)
        {
            Console.WriteLine("[ERROR] Producto agotado. No hay stock disponible.");
            return;
        }

        int cantidad;
        while (true)
        {
            cantidad = LeerEntero("Ingrese la cantidad a comprar: ", 1, int.MaxValue);
            if (cantidad > stockProductos[index])
            {
                Console.WriteLine($"[ERROR] Stock insuficiente. Solo quedan {stockProductos[index]} unidades en inventario.");
            }
            else
            {
                break;
            }
        }

        // Preguntar descuento frecuente
        bool aplicaDescuento = false;
        while (true)
        {
            Console.Write("¿Aplica descuento de cliente frecuente (10%)? (S/N): ");
            string respuesta = Console.ReadLine()?.Trim().ToUpper() ?? "";
            if (respuesta == "S")
            {
                aplicaDescuento = true;
                break;
            }
            else if (respuesta == "N")
            {
                aplicaDescuento = false;
                break;
            }
            Console.WriteLine("[ERROR] Responda 'S' para Sí o 'N' para No.");
        }

        // Calcular factura usando método obligatorio
        decimal montoIva, montoDescuento;
        decimal totalPagar = CalcularFactura(preciosProductos[index], cantidad, aplicaDescuento, out montoIva, out montoDescuento);
        decimal subtotal = preciosProductos[index] * cantidad;

        // Actualizar inventario y estadísticas
        stockProductos[index] -= cantidad;
        unidadesVendidasPorProducto[index] += cantidad;
        totalVentasRealizadas++;
        acumuladoCaja += totalPagar;
        montosVentas.Add(totalPagar);

        // Imprimir Ticket
        Console.WriteLine("\n====================================================");
        Console.WriteLine("                  TICKET DE VENTA");
        Console.WriteLine("====================================================");
        Console.WriteLine($" Producto:             {nombresProductos[index]} (x{cantidad})");
        Console.WriteLine($" Subtotal:             $ {subtotal,12:N2}");
        Console.WriteLine($" Descuento (10%):     -$ {montoDescuento,12:N2}");
        Console.WriteLine($" IVA (19%):            +$ {montoIva,12:N2}");
        Console.WriteLine(" ---------------------------------------------------");
        Console.WriteLine($" TOTAL A PAGAR:        $ {totalPagar,12:N2}");
        Console.WriteLine("====================================================");
        Console.WriteLine($"[OK] Venta efectuada con éxito. Stock actualizado: {stockProductos[index]} unidades.");
    }

    static void VerReporteCaja()
    {
        ImprimirEncabezado("REPORTE DE CAJA Y ESTADÍSTICAS DIARIAS");

        Console.WriteLine($"Total de ventas realizadas en la sesión: {totalVentasRealizadas}");
        Console.WriteLine($"Total acumulado ingresado a caja en dinero: $ {acumuladoCaja:N2}");

        decimal promedioVenta = totalVentasRealizadas > 0 ? acumuladoCaja / totalVentasRealizadas : 0m;
        Console.WriteLine($"Promedio de dinero por venta: $ {promedioVenta:N2}");

        // Encontrar producto con mayor cantidad de unidades vendidas
        if (nombresProductos.Count > 0 && totalVentasRealizadas > 0)
        {
            int maxUnidades = -1;
            string productoEstrella = "Ninguno";

            for (int i = 0; i < nombresProductos.Count; i++)
            {
                if (unidadesVendidasPorProducto[i] > maxUnidades)
                {
                    maxUnidades = unidadesVendidasPorProducto[i];
                    productoEstrella = nombresProductos[i];
                }
            }

            Console.WriteLine($"Producto más vendido: {productoEstrella} ({maxUnidades} unidades vendidas)");
        }
        else
        {
            Console.WriteLine("Producto más vendido: N/A (Aún no se han registrado ventas)");
        }
    }
}
