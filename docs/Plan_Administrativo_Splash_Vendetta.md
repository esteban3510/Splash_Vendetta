# Splash Vendetta — Plan Administrativo

**Metodología SUM (Scrum for Video Games) · Evidencia GA6-220501088-AA1-EV01 · Versión 1.0 · 1 de noviembre de 2026**

| Campo | Detalle |
|---|---|
| Proyecto | Splash Vendetta, shooter táctico 3D en primera persona para PC (Windows) |
| Responsable | Junior Esteban Benavides Arias (Ficha 3336515) |
| Equipo | Una persona, que asume los roles de diseño, desarrollo, pruebas y gestión |
| Herramientas | ClickUp (gestión Scrum), GitHub (repositorio), Notion (pruebas y seguimiento de hallazgos), Unity (motor) |

---

## 1. Resumen del estado del proyecto

| Fase | Resultado | Estado |
|---|---|---|
| Conceptualización | Documento de alto concepto, GDD v1.0, benchmarking y pitch | Completada |
| Prototipo vertical “El Refugio” | Mecánica nuclear, efectos visuales y sonoros, corrección de fallos | Completada |
| Iteración de validación | Enemigo básico, eliminación de objetivos, combate, pruebas con 5 jugadores y ajustes (Build V1.1) | Completada |
| Sprint 1 · Conceptualización y especificación | Concepto consolidado, plan administrativo, GDD v1.1 en el repositorio | En curso |

---

## 2. Cronograma

### 2.1 Cronograma general

| Hito | Inicio | Fin | Estado |
|---|---|---|---|
| Sprint 1 · Conceptualización, planificación y especificación | 2 nov 2026 | 4 nov 2026 | En curso |
| **Entrega 1** (GA6-220501088-AA1-EV01) | — | **5 nov 2026** | Pendiente |
| Sprint 2 · Diagrama de clases UML y pseudocódigo (propuesto) | 6 nov 2026 | 17 nov 2026 | Propuesto |
| Sprint 3 · Diagramas de secuencia e implementación en C# (propuesto) | 18 nov 2026 | 1 dic 2026 | Propuesto |
| Cierre: videos de sustentación y gameplay, build, PDF integrador (propuesto) | 2 dic 2026 | 3 dic 2026 | Propuesto |
| **Entrega 2** (GA6-220501088-AA2-EV01) | — | **4 dic 2026** | Pendiente |
| Fases 4 a 6 del proyecto (arte y audio finales, alfa y beta, lanzamiento) | 2027 | 2027 | Proyección tentativa, fuera del curso |

Los sprints 2 y 3 son una propuesta y se ajustarán en la planificación de cada sprint.

**Nota sobre el Sprint 1 en ClickUp:** la herramienta lo configuró con una semana completa (2 a 8 de noviembre). El compromiso del proyecto es cerrar todas las tareas el 4 de noviembre para poder entregar el 5. Las fechas límite de cada tarea reflejan ese compromiso.

### 2.2 Plan día a día del Sprint 1

| Fecha | Tareas planificadas | Puntos |
|---|---|---|
| 2 nov | Consolidar el concepto · Definir el alcance real del prototipo · Precisar público objetivo · Elaborar el cronograma | 4 |
| 3 nov | Elaborar el presupuesto · Elaborar la matriz de riesgos · Especificar la mecánica de disparo | 4 |
| 4 nov | Especificar y bocetar el proyectil de pintura · Migrar y actualizar el GDD en el repositorio · Revisión y retrospectiva | 5 |
| 5 nov | Armar el PDF integrador y verificar los enlaces · **Entrega** | — |

**Total comprometido:** 13 puntos de esfuerzo.

---

## 3. Presupuesto estimado

### 3.1 Supuestos

| Supuesto | Valor |
|---|---|
| Equivalencia de esfuerzo | 1 punto de esfuerzo ≈ 1,5 horas de trabajo |
| Valor de la hora de trabajo (referencia) | $15.000 COP |
| Horas del Sprint 1 | 13 puntos × 1,5 h ≈ 20 h |
| Horas estimadas de la Fase 3 (hasta el 4 dic) | 60 h |
| Conectividad y energía | $80.000 COP al mes, durante 2 meses |
| Contingencia | 10 % |

### 3.2 Presupuesto

| Concepto | Cantidad | Costo unitario (COP) | Total (COP) |
|---|---|---|---|
| Mano de obra · Sprint 1 | 20 h | 15.000 | 300.000 |
| Mano de obra · Fase 3 (Entrega 2) | 60 h | 15.000 | 900.000 |
| Conectividad y energía (nov y dic) | 2 meses | 80.000 | 160.000 |
| Licencias de software (Unity, GitHub, ClickUp, Notion, Excalidraw) | 5 | 0 | 0 |
| Equipo de cómputo (equipo propio) | 1 | 0 | 0 |
| Recursos de arte y audio (solo con licencia verificada) | — | 0 | 0 |
| **Subtotal** | | | **1.360.000** |
| Contingencia (10 %) | | | 136.000 |
| **Total estimado** | | | **1.496.000** |

**Nota:** el costo de licencias es cero porque se usan planes gratuitos, sujetos a sus condiciones de uso. Si algún recurso de terceros requiere pago, se añade en este apartado.

---

## 4. Análisis de riesgos

**Escala:** probabilidad (P) e impacto (I) de 1 a 3. Nivel = P × I. 1–2 bajo · 3–4 medio · 6–9 alto.

| ID | Riesgo | P | I | Nivel | Medida preventiva | Plan de contingencia |
|---|---|---|---|---|---|---|
| R1 | Tiempo insuficiente para entregar el 5 nov | 3 | 3 | 9 · Alto | Plan día a día, tareas pequeñas y revisión diaria del avance | Reducir la profundidad de las tareas menos críticas y priorizar los artefactos que piden los criterios |
| R2 | Los documentos de diseño quedan desfasados respecto al prototipo | 2 | 3 | 6 · Alto | Versionar el GDD en el repositorio y actualizarlo después de cada iteración | Revisión cruzada GDD ↔ prototipo antes de cada entrega |
| R3 | El plan gratuito de ClickUp limita puntos de esfuerzo o gráficos de sprint | 2 | 2 | 4 · Medio | Asignar puntos una sola vez, sin cambiarlos durante el sprint. Se activó la prueba gratuita de 15 días del plan Business para disponer del Sprint Burndown | Capturar el gráfico cada día. Si la prueba termina antes de lo previsto, registrar los puntos restantes reales y construir el gráfico de quemado con esos datos |
| R4 | Pérdida de archivos o de avances | 1 | 3 | 3 · Medio | Commits frecuentes y etiquetas de versión en GitHub | Restaurar desde el repositorio |
| R5 | Fallos del editor o del motor que bloqueen el trabajo | 2 | 2 | 4 · Medio | Mantener una versión estable del motor y documentar los fallos conocidos | Reiniciar el entorno y consultar el registro de errores resueltos |
| R6 | Crecimiento del alcance (agregar funciones no planificadas) | 2 | 3 | 6 · Alto | Alcance definido en el GDD; toda idea nueva entra primero al backlog | Posponer lo nuevo a un sprint posterior |
| R7 | Dependencia de una sola persona (enfermedad, falla del equipo) | 2 | 3 | 6 · Alto | Documentación al día y repositorio remoto | Trabajar desde otro equipo con el repositorio |
| R8 | Licencias dudosas en recursos de terceros | 2 | 2 | 4 · Medio | Usar solo recursos propios o con licencia verificada y registrar su origen | Reemplazar el recurso |
| R9 | El ejecutable no funciona en otro equipo | 1 | 3 | 3 · Medio | Probar el build en un segundo equipo antes de entregar | Publicar una versión corregida en el repositorio |
| R10 | El instructor no puede abrir los enlaces entregados | 2 | 3 | 6 · Alto | Abrir cada enlace en una ventana de incógnito antes de entregar | Corregir los permisos y reenviar el PDF |

---

## 5. Estructura del proyecto en ClickUp

| Elemento | Detalle |
|---|---|
| Espacio | Splash Vendetta |
| Carpeta de sprints | Sprints (con el complemento Sprints activado) |
| Lista de backlog | Backlog del proyecto |
| Sprint actual | Sprint 1 · Conceptualización (2 a 4 nov 2026) |
| Medida de costo | Puntos de esfuerzo (Sprint Points) |
| Etiquetas | Conceptualización · Planificación administrativa · Especificación · Elaboración (Entrega 2) |
