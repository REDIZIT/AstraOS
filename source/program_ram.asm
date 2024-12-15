section .data
test_string dd "RAM test started", 0
test_string2 dd "RAM test completed", 0
test_string3 dd "Hidden function called", 0

ram_64_max dq 0xFFFFFFFFFFFFFFFF  ; 16*F = 64 bit
ram_48_max dq 0xFFFFFFFFFFFF      ; 12*F = 48 bit
ram_32_max dq 0xFFFFFFFF          ; 8*F = 32 bit

ram_0 dq 0
ram_1 dq 1
ram_2 dq 2
ram_3 dq 3
ram_4 dq 4

ram_vga dq 0xB8000
;ram_64_max_truncated dq 0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF

const_value dq 12345


section .bbs
buffer resb 32


section .text
global program_ram_main

extern console_writeline
extern int_to_string
extern int_to_hex_string
extern int_to_bin_string

program_ram_main:

	push test_string
	call console_writeline
	add rsp, 4



	;mov qword [ram_64_max], 64
	;mov qword [ram_48_max], 48
	;mov qword [ram_32_max], 32

	;mov r10, ram_32_max
	;add r10, 1
	;mov qword [r10], 48

;	mov qword [ram_0], 40
;	mov qword [ram_0+4], 41
;
;	mov rdx, [ram_0+0]


;	mov byte [ram_0], 40
;
;	mov rax, [ram_0]
;	add rax, 4
;	mov byte [rax], 41
;
;	mov rdx, [ram_0]

	; Так делать нельзя (ошибка синтаксиса)
;	mov 0x00, 10
;	mov rdx, 0x00

	; Так можно
	mov rax, 0x00
	mov word [rax], 0xabcd

;	mov rax, 0x04
;	mov qword [rax], 11

;	mov rax, 0x00
;	add rax, 2
;	mov qword [rax], 42

;	mov rdx, program_ram_main
;	mov rbx, program_hidden
;	mov qword [rdx], program_hidden
;	jmp program_ram_main

	;
	; Hack the memory pointer
	;  - replace program_ram_main (for jmp) with another pointer (program_hidden)
	;  - when you try jump to program_ram_main you actually jump to program_hidden
	;
	mov rbx, program_ram_main
	;mov qword [rbx], 1075316840
	mov qword [rbx], 1075328104
	jmp program_ram_main
	mov rdx, [rbx]
	
	push rdx
	push buffer
	call int_to_string
	add rsp, 8

	push buffer
	call console_writeline
	add rsp, 4


	push test_string2
	call console_writeline
	add rsp, 4

	ret


program_hidden:
	push test_string3
	call console_writeline
	add rsp, 4
	hlt
	jmp program_hidden