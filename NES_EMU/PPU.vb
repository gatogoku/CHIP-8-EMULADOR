Imports Color = SFML.Graphics.Color

Public Class PPU
    'Sprite RAM (SPR-RAM) also known as OAM in the PPU.

    Public InternalMEM As List(Of Byte)  '16*1024 CHR ROM phisical Memory but can adress 63 kB
    Public OAM As List(Of Byte) '256 bytes of on-die object attribute memory (OAM)
    'max 25 colors used in the same scanline of 48
    Public SPR_RAM As List(Of Byte)




    Public Structure ConstantsValues
        'ADDRESSESS
        Public Shared NAME_TABLE1_INIT = &H2000
        Public Shared NAME_TABLE2_INIT = &H2400
        Public Shared NAME_TABLE3_INIT = &H2800
        Public Shared NAME_TABLE4_INIT = &H2C00

        Public Shared NAME_TABLE1_END = &H2399
        Public Shared NAME_TABLE2_END = &H2799
        Public Shared NAME_TABLE3_END = &H2BFF
        Public Shared NAME_TABLE4_END = &H3000

        Public Shared PATERN_TABLE_1_INIT = &H0
        Public Shared PATERN_TABLE_2_INIT = &H1000
        Public Shared PATERN_TABLE_1_END = &HFFF
        Public Shared PATERN_TABLE_2_END = &H1FFF
        '        A Pattern Table contains tile images (patterns) in the following format:
        'Character   Colors      Contents of Pattern Table
        '...*....    00010000    00010000 $10  +-> 00000000 $00
        '..O.O...    00202000    00000000 $00  |   00101000 $28
        '.#...#..    03000300    01000100 $44  |   01000100 $44
        'O.....O.    20000020    00000000 $00  |   10000010 $82
        '*******. -> 11111110 -> 11111110 $FE  |   00000000 $00
        'O.....O.    20000020    00000000 $00  |   10000010 $82
        '#.....#.    30000030    10000010 $82  |   10000010 $82
        '........    00000000    00000000 $00  |   00000000 $00
        '                            +---------+
        Public Shared PALETE_COLORS_1 = &H3F00
        Public Shared PALETE_COLORS_2 = &H3F10
        '        Palettes
        'There are two 16-byte Palettes. The one at $3F00 used for the picture. Another one, at $3F10, contains sprite colors. The $3F00 and $3F10 locations in VRAM mirror each other (i.e. it is the same memory cell) and define the background color of the picture.


        ' Attribute Table 3
        '--------------------------------------- $2FC0
        ' Attribute Table 2
        '--------------------------------------- $2BC0
        ' Attribute Table 1
        '--------------------------------------- $27C0
        ' Attribute Table 0
        '--------------------------------------- $23C0
        Public Shared ATTRIBUTE_TABLE0_INIT = &H23C0
        Public Shared ATTRIBUTE_TABLE1_INIT = &H27C0
        Public Shared ATTRIBUTE_TABLE2_INIT = &H2BC0
        Public Shared ATTRIBUTE_TABLE3_INIT = &H2FC0







    End Structure


    '    Sprites
    'There are 64 sprites which can be either 8x8 or 8x16 pixels. Sprite patterns are stored in one of the Pattern Tables in the PPU Memory. Sprite attributes are stored in a special Sprite Memory of 256 bytes which is not a part of neither CPU nor PPU address space. The entire contents of Sprite Memory can be written via DMA transfer using location $4014 (see above). Sprite Memory can also be accessed byte-by-byte by putting the starting address into $2003 and then writing/reading $2004 (the address will be incremented after each access). The format of sprite attributes is as follows:
    'Sprite Attribute RAM:
    '| Sprite#0 | Sprite#1 | ... | Sprite#62 | Sprite#63 |
    '     |          |
    '     +---- 4 bytes: 0: Y position of the left-top corner - 1
    '                    1: Sprite pattern number
    '                    2: Color and attributes:
    '                       bits 1,0: two upper bits of color
    '                       bits 2,3,4: Unknown (???)
    '                       bit 5: if 1, display sprite behind background
    '                       bit 6: if 1, flip sprite horizontally
    '                       bit 7: if 1, flip sprite vertically
    '                    3: X position of the left-top corner
    'Sprite patterns are fetched in the exactly same way as the tile patterns for the background picture. The only difference occurs in 8x16 sprites: the top half of a sprite is taken from the Sprite Pattern Table at $0000 while the bottom part is taken from the same location of $1000 Sprite Pattern Table.






    '    PPU address bits A10 and A11. It may set them in one of four possible ways:

    'A11 A10 Effect
    '----------------------------------------------------------
    ' 0   0  All four screen buffers are mapped to the same
    '        area of memory which repeats at $2000, $2400,
    '        $2800, and $2C00.
    ' 0   x  "Upper" and "lower" screen buffers are mapped to
    '        separate areas of memory at $2000, $2400 and
    '        $2800, $2C00.
    ' x   0  "Left" and "right" screen buffers are mapped to
    '        separate areas of memory at $2000, $2800 and
    '        $2400,$2C00. 
    ' x   x  All four screen buffers are mapped to separate
    '        areas of memory. In this case, the cartridge
    '        must contain 2kB of additional VRAM.






    '    Attribute Tables
    'Each Name Table has its own Attribute Table. A byte in this table represents a group of 4x4 tiles on screen which makes an 8x8 Attribute Table. Each 4x4 tile group is subdivided into four 2x2 squares as follows:

    '(0,0)  (1,0) 0|  (2,0)  (3,0) 1
    '(0,1)  (1,1)  |  (2,1)  (3,1)
    '--------------+----------------
    '(0,2)  (1,2) 2|  (2,2)  (3,2) 3
    '(0,3)  (1,3)  |  (2,3)  (3,3)
    'The attribute byte contains upper two bits of the color number for each 2x2 square (the lower two bits are stored in the Pattern Table):

    'Bits   Function                        Tiles
    '--------------------------------------------------------------
    '7,6    Upper color bits for square 3   (2,2),(3,2),(2,3),(3,3)    
    '5,4    Upper color bits for square 2   (0,2),(1,2),(0,3),(1,3)
    '3,2    Upper color bits for square 1   (2,0),(3,0),(2,1),(3,1)
    '1,0    Upper color bits for square 0   (0,0),(1,0),(0,1),(1,1)






    '   D. Pattern Tables
    '-----------------
    '  The Pattern Table contains the actual 8x8 tiles which the Name Table
    '  refers to. It also holds the lower two (2) bits of the 4-bit colour
    '  matrix needed to access all 16 colours of the NES palette. Example:

    '     VRAM    Contents of                     Colour 
    '     Addr   Pattern Table                    Result
    '    ------ ---------------                  --------
    '    $0000: %00010000 = $10 --+              ...1.... Periods are used to
    '      ..   %00000000 = $00   |              ..2.2... represent colour 0.
    '      ..   %01000100 = $44   |              .3...3.. Numbers represent
    '      ..   %00000000 = $00   +-- Bit 0      2.....2. the actual palette
    '      ..   %11111110 = $FE   |              1111111. colour #.
    '      ..   %00000000 = $00   |              2.....2.
    '      ..   %10000010 = $82   |              3.....3.
    '    $0007: %00000000 = $00 --+              ........

    '    $0008: %00000000 = $00 --+
    '      ..   %00101000 = $28   |
    '      ..   %01000100 = $44   |
    '      ..   %10000010 = $82   +-- Bit 1
    '      ..   %00000000 = $00   |
    '      ..   %10000010 = $82   |
    '      ..   %10000010 = $82   |
    '    $000F: %00000000 = $00 --+

    '  The result of the above Pattern Table is the character 'A', as shown
    '  in the "Colour Result" section in the upper right.






    '  E. Attribute Tables
    '-------------------
    '  The Pattern Table contains the actual 8x8 tiles which the Name Table
    '  refers to. It also holds the lower two (2) of the 4-bit colour matrix

    '  Each byte in an Attribute Table represents a 4x4 group of tiles on the
    '  screen. There's multiple ways to describe what the function of one (1)
    '  byte in the Attribute Table does:

    '    * Holds the upper two (2) bits of a 32x32 pixel grid, per 16x16 pixels.
    '    * Holds the upper two (2) bits of sixteen (16) 8x8 tiles.
    '    * Holds the upper two (2) bits of four (4) 4x4 tile grids.

    '  It's quite confusing; two graphical diagrams may help:

    '    +------------+------------+
    '    |  Square 0  |  Square 1  |  #0-F represents an 8x8 tile
    '    |   #0  #1   |   #4  #5   |
    '    |   #2  #3   |   #6  #7   |  Square [x] represents four (4) 8x8 tiles
    '    +------------+------------+   (i.e. a 16x16 pixel grid)
    '    |  Square 2  |  Square 3  |
    '    |   #8  #9   |   #C  #D   |
    '    |   #A  #B   |   #E  #F   |
    '    +------------+------------+

    '  The actual format of the attribute byte is the following (and corris-
    '  ponds to the above example):

    '     Attribute Byte
    '       (Square #)
    '    ----------------
    '        33221100
    '        ||||||+--- Upper two (2) colour bits for Square 0 (Tiles #0,1,2,3)
    '        ||||+----- Upper two (2) colour bits for Square 1 (Tiles #4,5,6,7)
    '        ||+------- Upper two (2) colour bits for Square 2 (Tiles #8,9,A,B)
    '        +--------- Upper two (2) colour bits for Square 3 (Tiles #C,D,E,F)







    Public Structure Sprite
        Public Tile(,) As Integer 'Public TileNumber As Integer ' En la realidad el tile no sera un array sino un numero que hace referencia a una posicion de un array de tiles
        Public Atributos As Integer
        '76543210
        '|||   ++- Color Palette of sprite.  Choose which set of 4 from the 16 colors to use
        '||+------ Priority (0: in front of background; 1: behind background)
        '|+------- Flip sprite horizontally
        '+-------- Flip sprite vertically
        Public pX As Integer
        Public pY As Integer
        Public Sub New(p1(,) As Integer, p2 As Integer, p3 As Integer, p4 As Integer)
            Tile = p1 : Atributos = p2 : pX = p3 : pY = p4
        End Sub
    End Structure


    'Each sprite object entry in the OAM uses 4 bytes as follows:
    '1. Y-coordinate
    '2. Tile index in the pattern table
    '3. Attributes • Two ﬁrst bits are the msb of color oﬀset • Bit 5 is priority with respect to the background • Bit 6 is used for horizontal ﬂipping • Bit 7 is used for vertical ﬂipping 4. X-coordinate

    'For each clock cycle the CPU runs, the PPU runs three cycles.

    'All the communication between the CPU and PPU takes place over the I/O-mapped registers and this is the only way the programmer can receive status information and control the PPU’s behaviour. As previously mentioned these registers reside at locations 0x2000-0x2007.


    'The PPU also known as 2C02 is running approximately three times faster than the CPU depending on the region which equals to 5.37MHz on a NTSC system. The static procedure of displaying frames is executed 60 times per second where each frame consists of 240 visible scanlines which in turn have 256 pixels each. Like the CPU the 2C02 can also address 64KB of memory but it only has 16KB of video ram (VRAM) used for storing graphics related content such as palettes, patterns and name tables.

    '8 registros $2000-$2007 se reflejan en cada 8 bytes desde $ 2008 hasta $ 3FFF, por lo que una escritura a $ 3456 es lo mismo que una escritura a $ 2006.


    Public PPUCTRL As Byte 'permiso escritura despues de 30,000 ciclos. desde el reset
    ' VPHB SINN
    '|||| || ++ - Dirección de nombre base (0 = $ 2000; 1 = $ 2400; 2 = $ 2800; 3 = $ 2C00)
    '|||| | + --- Incremento de dirección VRAM por CPU lectura / escritura de PPUDATA (0: agregue 1, pasando; 1: agregue 32, bajando)
    '|||| + ----- Dirección de tabla de patrón de Sprite para sprites 8x8  (0: $ 0000; 1: $ 1000; ignorado en el modo 8x16)
    '||| + ------ Dirección de la tabla de patrones de fondo (0: $ 0000; 1: $ 1000)
    '|| + ------- Tamaño de Sprite (0: 8x8 píxeles; 1: 8x16 píxeles)
    '| + -------- PPU master / slave select
    '| +--------- (0: leer el telón de fondo de los pines EXT; 1: color de salida en los pines EXT)  Genera un NMI al inicio del   intervalo de supresión vertical (0: apagado; 1: encendido)


    Public PPUMASK As Byte 'permiso escritura
    '    BGRs bMmG
    '|||| ||||
    '|||| ||| + - Escala de grises (0: color normal, 1: produce una pantalla en escala de grises)
    '|||| || + -- 1: Mostrar fondo en la parte más a la izquierda 8 píxeles de la pantalla, 0: Ocultar
    '|||| | + --- 1: Mostrar sprites en la parte más a la izquierda 8 píxeles de la pantalla, 0: Ocultar
    '|||| + ----- 1: Mostrar fondo
    '||| + ------ 1: Mostrar sprites
    '|| + ------- Destacar rojo
    '| + -------- Enfatiza el verde
    '+ --------- Enfatizar azul
    Public PPUSTATUS As Byte 'permiso lectura
    '    VSO. ....
    '|||| ||||
    '||| + - ++++ - Los bits menos significativos escritos previamente en un registro PPU (Debido a que el registro no está siendo actualizado para esta dirección)
    '|| + ------- Desbordamiento de Sprite. La intención era establecer esta bandera. siempre que más de ocho sprites aparecen en una línea de exploración, pero un
    '|| error de hardware hace que el comportamiento real sea más complicado y generar falsos positivos así como falsos negativos; ver PPU sprite de evaluación . Esta bandera se establece durante el sprite
    '|| evaluación y borrado en el punto 1 (el segundo punto) del
    '|| Línea de pre-render.
    '| + -------- Sprite 0 Hit. Se establece cuando un píxel distinto de cero del sprite 0 se superpone
    '| un píxel de fondo distinto de cero; borrado en el punto 1 del pre-render
    '| línea. Se utiliza para la sincronización de trama.
    '+ --------- Se ha iniciado el blanco vertical (0: no en vblank; 1: en vblank).  Establecido en el punto 1 de la línea 241 (la línea * después de * el post-render  línea); Despejado después de leer $ 2002 y en el punto 1 de la Línea de pre-render.
    Public OAMADDR As Byte 'permiso escritura


    Public OAMDATA As Byte
    Public PPUSCROLL As Byte
    Public PPUADDR As Byte
    Public PPUDATA As Byte
    Public OAMDMA As Byte




    Public Function POWER_UP_STATE()
        '? = unknown, x = irrelevant, + = often set, U = unchanged
        PPUCTRL = &H0
        PPUMASK = &H0
        ' PPUSTATUS = '+0+xxxxx'
        'OAMADDR ($2003)	$00	
        '$2005 / $2006 latch	cleared
        PPUSCROLL = &H0
        PPUADDR = &H0
        PPUDATA = &H0

    End Function


    '    Public Function RESET_STATE()
    '        PPUCTRL ($2000)		0000 0000
    'PPUMASK ($2001)		0000 0000
    'PPUSTATUS ($2002)		U??x xxxx
    'OAMADDR ($2003)		unchanged1
    '$2005 / $2006 latch	cleared
    'PPUSCROLL ($2005)	$0000
    'PPUADDR ($2006)	unchanged
    'PPUDATA ($2007) read buffer	$00
    '    End Function





    '    Aquí hay una breve descripción de lo que hacen todos los módulos: 14 MODULOS

    'nes_iface.v: decodifica lecturas / escrituras de la CPU en solicitudes de lectura / escritura para el PPU. Al interactuar con el bus PPU a través de FIFOs asíncronos en ppubridge.v de nivel superior, este módulo no es estrictamente necesario. Actúa principalmente como un paso a través ahora.

    'regs.v: Muchas funciones se implementan aquí. Aquí se realiza toda la decodificación de direcciones para los ocho registros asignados en memoria, y los que corresponden a flip-flops físicos se crean instancias aquí. Todo el acceso desde la CPU se maneja aquí, incluidas las lecturas y escrituras en VRAM. Dado que la CPU y los módulos de representación comparten VRAM, el arbitraje también se realiza en este módulo. Los dos módulos de representación que comparten la VRAM (bg_fetch y sprite_fetch) solicitan una dirección para leer, y esa dirección se reenvía a VRAM desde regs.v. Finalmente, los contadores para dibujar mosaicos de fondo también se almacenan aquí, ya que están estrechamente acoplados a la dirección VRAM. Cuando bg_fetch realiza una solicitud para leer VRAM, estos contadores de direcciones internas se incrementan.

    'vidmem.v: Aquí se crean instancias de dos RAM de bloque: una para las tablas de patrones de NES y otra para las tablas de nombre y atributo. La decodificación de la dirección se realiza aquí para elegir la RAM de bloque adecuada de acuerdo con la dirección. Las tablas de patrones almacenan los mapas de bits reales utilizados para representar tanto los sprites como los fondos. Las Tablas de nombres (y las Tablas de atributos, que son solo apéndices de 64 bytes al final de cada Tabla de nombres) almacenan un índice (un puntero) en las Tablas de patrones para cada uno de los mosaicos de fondo de 32x30 en la pantalla y, por lo tanto, solo son relevantes para dibujo de fondo Como en la NES original, las Tablas de patrones son en realidad ROM, los contenidos de esa memoria se inicializan previamente con un archivo .hex. El mapa de memoria es el siguiente: las Tablas de patrón 0 y 1 toman las direcciones 0x0000-0x1FFF y las Tablas de nombres 1-4 toman las direcciones 0x2000-0x2FFF. Solo hay suficiente memoria física para dos tablas de nombres, por lo que las otras dos tablas de nombres son simplemente reflejos de las dos primeras. Toda la memoria es de 8 bits de ancho.

    'spr_ram.v: Almacena X, Y, Número de mosaico y Atributos especiales (4 bytes en total) para cada uno de los 64 sprites dibujables por el PPU. Estos valores son llenados por la CPU del host a través de regs.v.

    'in_range_evalulate.v: Contiene un contador para iterar a través de los 64 sprites posibles y acceder a sus contenidos desde Sprite Ram. Este proceso ocurre una vez por línea de escaneo, para determinar qué sprites son visibles para dibujar durante la SIGUIENTE línea de escaneo.

    'spr_evaluator.v: Para cada sprite iterado por in_range_evaluator, esto compara la coordenada y de ese sprite con la coordenada y actual para ver si debe dibujarse durante la siguiente línea de escaneo. Se pueden reconocer hasta ocho de estos sprites, y otros 4 valores de Sprite Ram se descargan en la memoria temporal de Sprite:

    'spr_tempmem.v: Esto es solo un bloque de ram distribuido para almacenar hasta ocho sprites encontrados por el proceso de evaluación de sprites en cada línea de escaneo. Los resultados (sprite x, sprite y, atributos, número de mosaico) se compactan en palabras de 25 bits.

    'spr_fetcher.v: Durante cada línea de escaneo, esto leerá el contenido de la memoria temporal del sprite (se llenará durante la línea de escaneo ANTERIOR) y recuperará todos los datos de mapa de bits de hasta ocho sprites, y los almacenará en los Buffers de Sprite.

    'spr_buffers.v: Hay ocho buffers instanciados aquí. Cada búfer es un registro de desplazamiento de 4 bits de ancho y 16 bits de profundidad, que contiene píxeles de 4 bits que deben desplazarse. Cuando la posición X actual en la pantalla alcanza la posición X inicial del sprite, los bits se desplazan de uno en uno. Un contador dentro de cada búfer cuenta atrás hasta que este tiempo pasa. Dado que los ocho sprites pueden superponerse, hay un mux de prioridad para priorizar la salida de un registro de desplazamiento sobre el otro (esto se hace sobre la base del índice de sprite 0-63 como se estableció originalmente en Sprite RAM).

    'bg_fetch.v: Análogo a spr_fetcher.v, pero esto obtiene mapas de bits de mosaico de fondo. Para determinar qué mapas de bits de sprite se van a buscar, primero realiza una lectura en la Tabla de nombres y utiliza el valor devuelto como puntero en una Tabla de patrones para buscar el sprite. El contenido del sprite se vuelca en el Buffer BG.

    'bg_buffer.v: Al igual que los búferes de sprites, esto contiene registros de desplazamiento de 4 bits de ancho para desplazar cada píxel de 4 bits a medida que la pantalla se actualiza horizontalmente.

    'composite_mux.v: el píxel de fondo actual que se está dibujando y el píxel de sprite actual se priorizan unos sobre otros aquí en función de una bandera para cada sprite que dice si se debe dibujar delante o detrás del fondo. Su salida es un valor de 4 bits que es "el" píxel que se dibujará durante este ciclo.

    'palette.v: el píxel de 4 bits generado por el canal de procesamiento se convierte en un índice de 6 bits en la paleta de colores de televisión NTSC (que se resolverá en valores reales RGB dentro del módulo VGA-VGA de nivel superior). Por lo tanto, cada paleta contiene 16 entradas, que son programables por el usuario para apuntar a colores NTSC de 6 bits específicos mediante escrituras desde la CPU (nuevamente realizadas a través de regs.v).

    'main.v: contiene dos contadores, uno para X y otro para Y, que están contando e indicando constantemente el píxel que se está dibujando en la pantalla. Estos contadores se utilizan para generar una dirección en el framebuffer para escribir un píxel (el valor de color del píxel sale de la paleta durante el mismo ciclo). Además, ciertos valores / rangos de X e Y activarán las señales de control para iniciar / detener / restablecer las diversas partes de la tubería de renderizado tanto para los sprites como para el fondo. En esencia, esta es una máquina de estados con una gran cantidad de estados. La señal de interrupción de retroceso vertical se mantiene alta durante las últimas 20 líneas de exploración de la pantalla.

















End Class
