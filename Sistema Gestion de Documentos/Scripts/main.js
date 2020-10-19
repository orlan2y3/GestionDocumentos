const btnDelete = document.querySelectorAll('.btn-delete')

if (btnDelete) {
    const btnArray = Array.from(btnDelete);
    btnArray.forEach((btn) => {
        btn.addEventListener('click', (e) => {
            if (!confirm('Seguro que desea eliminar ese registro ?')) {
                e.preventDefault();
            }
        });
    });
}

$(document).ready(function () {             //Ejecutar cuando todo el documento este cargado

    //Para llenar el select con los remitentes
    $('#remitente').keyup(function () {     //Disparar cuando se suelte la tecla en el control que tiene el id remitente

        var remi = $("#remitente").val();
        remi = remi.trim();
        if (remi.length == 0 || remi.length < 3) {
            $("#select_remitente").empty();
        }
        else if (remi.length > 2) {
            
            var remitente = $("#remitente").val()   //Tomar el valor que esta en el control con el id = remitente
            
            //preparar datos json para enviar al backend
            var json = {"valor": remitente};

            //Ejecutar ajax
            $.ajax({
                url: "/Home/Remitente",      //Enviar al backend a la ruta remitente          
                type: "POST",           //Metodo a usar
                dataType: "json",   //Tipo de datos a usar
                data: JSON.stringify(json), //Convertir a json                
                contentType: "application/json",
                success: function (data, textStatus, jqXHR) {                    
                    $("#select_remitente").empty();
                    if (data != "") {
                    $.each(data, function (key, registro) {
                        $("#select_remitente").append('<option value=' + data[key].per_id + '>' + data[key].nombre + '</option>');  //Llenar select                       
                        });
                    }
                },

                error: function (jqXHR, textStatus, errorThrown) {
                    console.log(errorThrown);
                }

            });

        }

    });

    //Para llenar el select con las instituciones
    $('#institucion').keyup(function () {

        var inst = $("#institucion").val();
        inst = inst.trim();
        if (inst.length == 0 || inst.length < 3) {
            $("#select_institucion").empty();
        }
        else if (inst.length > 2) {
            
            var institucion = $("#institucion").val()
            var json = { "valor": institucion };

            $.ajax({
                url: "/Home/Institucion",
                type: "POST",
                dataType: "json",
                data: JSON.stringify(json),
                contentType: "application/json",
                success: function (data, textStatus, jqXHR) {
                    $("#select_institucion").empty();
                    if (data != "") {
                        $.each(data, function (key, registro) {
                            $("#select_institucion").append('<option value=' + data[key].cre_id + '>' + data[key].nombre_institucional + '</option>');
                        });
                    }
                },

                error: function (jqXHR, textStatus, errorThrown) {
                    console.log(errorThrown);
                }

            });

        }

    });


    //Para llenar el select con las Sub Series
    $('#select_serie').change(function () {

        var serie = $("#select_serie").val();
        if (serie == 0) {
            $("#select_sub_serie").empty();
        }
        else {

            var serie = $("#select_serie").val() 
            var json = { "valor": serie };

            $.ajax({
                url: "/Home/Serie",
                type: "POST",
                dataType: "json",
                data: JSON.stringify(json),
                contentType: "application/json",
                success: function (data, textStatus, jqXHR) {
                    $("#select_sub_serie").empty();

                    $.each(data, function (key, registro) {
                        if (registro != 'Missing data!') {
                            $("#select_sub_serie").append('<option value=' + data[key].sub_id + '>' + data[key].titulo + '</option>');
                        }
                    });

                },
                error: function (jqXHR, textStatus, errorThrown) {
                    console.log(errorThrown);
                }

            });

        }

    });


    //Para validar que seleccione el tipo del documento ***********
    $("#formulario").submit(function (event) {
        var resul = $('input:radio[name=tipo]:checked').val()
        if (resul) {
            return true
        }
        else {
            alert("Debe seleccionar el tipo de documento");
            return false
        }
    });

});
//*******************************************************************


//Para grabar nuevo remitente con el formulario modal ***********
$("#formulario_remi").submit(function (event) {
    event.preventDefault()

    var rem = $("#remitente_modal").val()
    var rol = $("#rol_modal").val()
  
    var json = {
        "remitente": rem,
        "rol": rol
    };

    $.ajax({
        url: "/Home/grabar_remitente_modal",
        type: "POST",
        dataType: "json",
        data: JSON.stringify(json),
        contentType: "application/json",
        success: function (data, textStatus, jqXHR) {
            $("#remitente_modal").empty();
            $("#rol_modal").empty();
            
                if (data != "Missing data!") {
                    swal("Ok!", "Remitente Grabado!", "success");
                }
                else
                {
                    swal("Error", "No fue posible grabar el remitente", "error");
                }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.log(errorThrown);
        }
    });
});
//*****************************************************

//Para grabar nueva institución con el formulario modal ***********
$("#formulario_inst").submit(function (event) {
    event.preventDefault()

    var inst = $("#institucion_modal").val()
    var siglas = $("#siglas_modal").val()
    var dir = $("#dir_modal").val()
    var email = $("#email_modal").val()
    var tel = $("#tel_modal").val()

    var json = {
        "institucion": inst,
        "siglas": siglas,
        "direccion": dir,
        "email": email,
        "telefono": tel
    };

    $.ajax({
        url: "/Home/grabar_institucion_modal",
        type: "POST",
        dataType: "json",
        data: JSON.stringify(json),
        contentType: "application/json",
        success: function (data, textStatus, jqXHR) {
            $("#institucion_modal").empty();
            $("#siglas_modal").empty();
            $("#dir_modal").empty();
            $("#email_modal").empty();
            $("#tel_modal").empty();
            
            if (data != "Missing data!") {
                swal("Ok!", "Institucion Grabada!", "success");
            }
            else
            {
                swal("Error", "No fue posible grabar la institución", "error");
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.log(errorThrown);
        }
    });
});


// ********************************************************************************************************************
// Scripts de consulta.html 
$(document).ready(function () { //Ejecutar cuando todo el documento este cargado
    $('#remitente2').keyup(function () { //Disparar cuando se suelte la tecla en el control que tiene el id remitente
        var remi = $("#remitente2").val(); remi = remi.trim(); if (remi.length == 0 || remi.length < 3) {
            $("#select_remitente2").empty();
        } else if (remi.length > 2) {

            var remitente = $("#remitente2").val()   //Tomar el valor que esta en el control con el id = remitente

            //preparar  json para enviar al backend
            var json = { "valor": remitente };

            //Ejecutar ajax
            $.ajax({
                url: "/Home/Remitente",     //Enviar al backend a la ruta remitente
                type: "POST",           //Metodo a usar
                dataType: "json",   //Tipo de datos a usar
                data: JSON.stringify(json), //Convertir a json
                contentType: "application/json",
                success: function (data, textStatus, jqXHR) {
                    $("#select_remitente2").empty();
                    $.each(data, function (key, registro) {
                        $("#select_remitente2").append('<option value=' + data[key].per_id + '>' + data[key].nombre + '</option>');  //Llenar select
                    });
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    console.log(errorThrown);
                }

            });

        }

    });


    //Para llenar el select con las instituciones
    $('#institucion2').keyup(function () {

        var inst = $("#institucion2").val();
        inst = inst.trim();
        if (inst.length == 0 || inst.length < 3) {
            $("#select_institucion2").empty();
        }
        else if (inst.length > 2) {

            var institucion = $("#institucion2").val()

            var json = { "valor": institucion };

            $.ajax({
                url: "/Home/Institucion",
                type: "POST",
                dataType: "json",
                data: JSON.stringify(json),
                contentType: "application/json",
                success: function (data, textStatus, jqXHR) {
                    $("#select_institucion2").empty();
                    $.each(data, function (key, registro) {
                        $("#select_institucion2").append('<option value=' + data[key].cre_id + '>' + data[key].nombre_institucional + '</option>');
                    });
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    console.log(errorThrown);
                }

            });

        }

    });

});

function PreguntarSalir() {
    if (confirm('Seguro que desea salir del programa ?')) {
        return true;
    }
    else {
        return false;
    }
}

function PreguntarEliminar() {
    if (confirm('Seguro que desea eliminar ese registro ?')) {
        return true;
    }
    else {
        return false;
    }
}

//Funcion para mantener activa la seccion de forma permanente
//var myVar = setInterval(EnviarDatos, 900000);

//function EnviarDatos() {
//    var json = { "valor": "" };

//    $.ajax({
//        url: "/Home/MantenerSeccionActiva",
//        type: "POST",
//        dataType: "json",
//        data: JSON.stringify(json),
//        contentType: "application/json",
//        success: function (data, textStatus, jqXHR) {
//             //if (data != "Missing data!") {
//            //    swal("Ok!", "Dato enviado al servidor!", "success");
//            //}
//            //else {
//            //    swal("Error", "Enviando dato", "error");
//            //}
//        },
//        error: function (jqXHR, textStatus, errorThrown) {
//            window.location.href = "/Home/Login";
//            //console.log(errorThrown);
//            //swal("Error", "Session cerrada volver a loguearse", "error");
//        }
//    });
//}

