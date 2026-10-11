1\. Información de la versión probada

Nombre o identificador de la build, Release, commit o versión.

2\. Pruebas funcionales T1–T6

Setup, pasos, esperado, real, Pass/Fail y evidencia en cada prueba.

3\. Pruebas con participantes P01–P03

Ambos escenarios, tiempos, finalización, errores, observaciones y ayuda.

4\. Problema de usabilidad 1

Evidencia inicial, cambio implementado y resultado posterior.

5\. Problema de usabilidad 2

Evidencia inicial, cambio implementado y resultado posterior.

6\. Limitaciones y problemas no resueltos

Describir honestamente qué sigue fallando o qué no se pudo comprobar.

TESTS QUE DEBEMOS HACER NOSOTROS 3

| ID | Prueba | ¿Qué deben comprobar? |
| :---: | :---: | :---: |

| T1 | Checkout | Se completa el préstamo y se muestran la ubicación y la fecha límite correctas. |
| :---- | :---- | :---- |

| T2 | No availability | Se explica que no hay kits y existe un siguiente paso funcional. |
| :---- | :---- | :---- |

| T3 | Invalid kit | Se bloquea un kit inválido o no disponible; no se permite confirmar. |
| :---- | :---- | :---- |

| T4 | Editing choices | Se pueden editar las elecciones sin perder información válida. |
| :---- | :---- | :---- |

| T5 | Persistence | Los préstamos y el inventario siguen correctos después de reiniciar la aplicación. |
| :---- | :---- | :---- |

| T6 | Scenario reset | Los controles permiten preparar y volver a probar los dos escenarios. |
| :---- | :---- | :---- |

Para cada una se tiene que registrar:

* Preparación inicial o setup.  
* Pasos ejecutados.  
* Resultado esperado.  
* Resultado real.  
* Estado: Pass o Fail.  
* Evidencia, por ejemplo, capturas de pantalla.

También debemos identificar exactamente qué build, Release, commit o versión probamos. No poner solamente “se probó la última versión”.

TESTS EXTERNOS DE 3 PERSONAS NO DEL EQUIPO  
Cada persona debe intentar los dos escenarios sin que nosotros les vayamos indicando dónde dar clic.

