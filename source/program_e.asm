section .data
	label dd "Ticks since startup:", 0  ; DON'T FORGET ABOUT ZERO-END
	number dq 0

section .bss
	buffer resb 32

section .text
global program_e_main

extern print_string_stack_coord_locals
extern int_to_string

program_e_main:


.loop:
	mov rbx, [number]
	add rbx, 1
	mov [number], rbx

	push qword [number]
	push buffer
	call int_to_string
	add rsp, 8


	push buffer
	push 2+21
	push 1
	call print_string_stack_coord_locals
	add rsp, 12

	push label
	push 2
	push 1
	call print_string_stack_coord_locals
	add rsp, 12

	;hlt
	jmp .loop

	ret