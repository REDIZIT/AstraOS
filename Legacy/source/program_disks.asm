section .data
	msg1 db "main", 0
	msg2 db "after ATA, waiting response", 0
	msg3 db "disk is ready", 0
	msg4 db "data is ready", 0
	msg5 db "read is done", 0
	msg6 db "return", 0
	msg_serial db "Serial Number: ", 0
    msg_model db "Model: ", 0
    msg_size db "Size (sectors): ", 0
	newline db 0x0D, 0x0A, 0

section .bss
	buffer resb 512  ; Буфер для чтения информации с диска
	tostring_buffer resb 32


section .text
global program_disks_main

extern console_writeline
extern int_to_string
extern int_to_hex_string
extern int_to_bin_string
extern slice_buffer
extern print_string_stack_coord_locals

program_disks_main:


; Slice buffer tests. Working.

;	push msg2
;	push tostring_buffer
;	push 2
;	push 10
;	call slice_buffer
;	add rsp, 16
;
;	push msg2
;	call console_writeline
;	add rsp, 4
;
;	push tostring_buffer
;	call console_writeline
;	add rsp, 4

;	ret



	push msg1
	call console_writeline
	add rsp, 4


	; Указываем порт команды для первичного канала ATA
	mov dx, 0x1F6       ; Регистр выбора диска/драйва
	mov al, 0xA0        ; Выбираем Master диск (0xB0 для Slave)
	out dx, al


	push msg2
	call console_writeline
	add rsp, 4

	; Ожидаем, пока диск готов
.wait_busy:
	mov dx, 0x1F7       ; Регистр статуса
	in al, dx
	test al, 0x80       ; Проверяем флаг занятости (bit 7)
	jnz .wait_busy


	push msg3
	call console_writeline
	add rsp, 4


	; Отправляем команду IDENTIFY (0xEC)
	mov dx, 0x1F7
	mov al, 0xEC        ; Команда IDENTIFY
	out dx, al

	; Ждем готовности данных
.wait_data_ready:
	in al, dx
	test al, 0x08       ; Проверяем флаг DRQ (Data Request, bit 3)
	jz .wait_data_ready


	push msg4
	call console_writeline
	add rsp, 4


	; Читаем 256 слов (512 байт) данных из регистра данных
	mov rdx, 0x1F0       ; Data port (256 16-bit values)
	lea rdi, [buffer]    ; Адрес буфера
	mov cx, 256         ; 256 слов (по 2 байта каждое)
.read_data:
	in ax, dx           ; Читаем слово из порта
	stosw               ; Записываем в буфер
	loop .read_data


	push msg5
	call console_writeline
	add rsp, 4


	;
	; Работа с ответом Identify
	;

;	; Получаем серию (10-19)
;	push buffer
;	push tostring_buffer
;	push 10 ; start index
;	push 10 ; len
;	call slice_buffer
;	add rsp, 16
;
;	mov rbx, tostring_buffer
;	add rbx, 11  ; len + 1
;	mov byte [rbx], 0  ; add endzero
;
;;	mov byte [tostring_buffer], "F"
;
;	push tostring_buffer
;	call console_writeline
;	add rsp, 4

	; 4. Проверяем слово 217 (ротационные характеристики)
;    mov rdi, buffer
;    add rdi, 217 * 2           ; Смещение до слова 217 (умножаем на 2, т.к. слово — 2 байта)
;    mov rbx, 0
;    mov bl, [rdi]              ; Считываем значение

	push 12345
	push buffer
	call int_to_string
	add rsp, 8

	push buffer  ; "12345"
	push 0
	push 0
	call print_string_stack_coord_locals
	add rsp, 12

;    mov rdx, 65
;
;    push rdx
;    push tostring_buffer
;    call int_to_hex_string
;    add rsp, 8
    
    push tostring_buffer
    call console_writeline
	add rsp, 4


	; return with message
	;
	push msg6
	call console_writeline
	add rsp, 4
	ret


	lea si, [msg_serial]
    call print_string

    lea si, [buffer + 20] ; Серийный номер начинается с offset 20 (10 слов)
    mov cx, 20           ; Длина серийного номера = 10 слов = 20 байт
    call print_serial_or_model

    ret

    ; Вывод модели диска
    lea si, [msg_model]
    call print_string

    lea si, [buffer + 54] ; Модель начинается с offset 54 (20 слов)
    mov cx, 40           ; Длина модели = 20 слов = 40 байт
    call print_serial_or_model

    ; Вывод размера диска
    lea si, [msg_size]
    call print_string

    ; Чтение размера из offset 60 (2 слова)
    mov ax, [buffer + 60]
    mov dx, [buffer + 62]
    call print_size



	push msg6
	call console_writeline
	add rsp, 4

	ret




; -------------------------------
; Функции вывода на экран
; -------------------------------

; Вывод строки (null-terminated) на экран
print_string:
    mov ah, 0x0E         ; BIOS функция вывода символа в текстовом режиме
.print_char:
    lodsb                ; Читаем байт из [SI]
    or al, al            ; Проверяем конец строки (null-terminated)
    jz .done
    int 0x10             ; Выводим символ
    jmp .print_char
.done:
    ret

; Вывод данных (серийного номера или модели)
print_serial_or_model:
    ; Читает CX байтов из [SI] и выводит
.next_char:
    lodsb                ; Читаем байт
    cmp al, ' '          ; Убираем лишние пробелы (опционально)
    jne .print
    mov al, '_'          ; Заменяем пробел на _
.print:
    mov ah, 0x0E
    int 0x10
    loop .next_char
    call print_newline
    ret

; Вывод размера диска (в блоках секторов)
print_size:
    ; AX = младшие 16 бит, DX = старшие 16 бит
    push ax
    push dx

    ; Преобразуем число в строку (десятичное)
    mov bx, 10
    xor cx, cx
.next_digit:
    xor dx, dx
    div bx                ; DX:AX / 10 -> AX = результат, DX = остаток
    push dx               ; Сохраняем остаток (цифру)
    inc cx                ; Увеличиваем количество цифр
    test ax, ax
    jnz .next_digit

    ; Выводим цифры
.print_digits:
    pop ax
    add al, '0'
    mov ah, 0x0E
    int 0x10
    loop .print_digits

    call print_newline
    pop dx
    pop ax
    ret

; Вывод новой строки
print_newline:
    lea si, [newline]
    call print_string
    ret