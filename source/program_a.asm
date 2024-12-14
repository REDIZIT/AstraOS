section .text
global program_a_main

program_a_main:
	mov rdi, 0xB8000
	call print_test_string

	mov rdi, 0xB8000
	add rdi, 160
	call print_test2_string

	mov rdi, 0xB8000
	add rdi, 160 * 4
	call print_test3_string
	ret

;
; Functions
;
print_test_string:
	mov rsi, message
	call print_string
	ret


print_test2_string:
	mov rsi, message2
	call print_string
	ret

print_test3_string:
	mov rsi, message3
	call print_string
	ret



; Функция вывода строки
print_string:
	mov rcx, -1
.print_loop:
	lodsb
	test al, al
	jz .done
	mov [rdi], al
	mov byte [rdi+1], 0x07
	add rdi, 2
	loop .print_loop
.done:
	ret


section .data
message db "Hello, Multiboot World!", 0
message2 db "Another string", 0
message3 db "One more time but split and as program", 0
result db "0000000000", 0 ; Буфер для строки результата (максимум 10 цифр)