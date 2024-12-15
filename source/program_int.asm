global program_int_main
extern print_string_stack_coord_locals
extern console_writeline
extern int_to_string
extern int_to_hex_string
extern int_to_bin_string


section .data
    msg_key_pressed db "Key pressed :", 0
    msg_key_released db "Key released:", 0

section .bbs
	buffer resb 32

section .text


program_int_main:
	; Инициализация клавиатурного контроллера
    ; Переходим в бесконечный цикл, ожидающий нажатия клавиши

check_key:
    ; Проверяем, готов ли клавиатурный контроллер (проверка на доступность данных)
    in al, 0x64                ; Читаем состояние клавиатурного контроллера
    test al, 0x01              ; Проверяем флаг Output Buffer Full (бит 0)
    jz check_key               ; Если данных нет, повторяем проверку

    ; Если данные доступны, считываем с порта 0x60
    ;mov rax, 0
    in al, 0x60                ; Чтение кода клавиши из порта 0x60
    push rax
    call on_any_key_pressed         ; Печатаем сообщение
    add rsp, 4

    ; Возвращаемся к проверке нажатий
    jmp check_key

; (+4) key_code
on_any_key_pressed:

	mov byte rdx, [rsp+4]

	test rdx, 0b10000000
	jz .j_on_key_pressed
	jmp .j_on_key_released


.j_on_key_pressed:
	push msg_key_pressed
	call console_writeline
	add rsp, 4
	jmp .j_exit

.j_on_key_released:
	push msg_key_released
	call console_writeline
	add rsp, 4
	jmp .j_exit

.j_exit:

	mov byte rdx, [rsp+4]

	push rdx
	push buffer
	call int_to_hex_string
	add rsp, 8

	push buffer
	call console_writeline
	add rsp, 4

    ret