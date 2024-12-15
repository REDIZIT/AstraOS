section .data
	msg_old dd "Old way used", 0
	msg_proper dd "Proper way used", 0

section .bbs
	buffer resb 32


section .text
global program_proper_calls_main

extern console_writeline
extern int_to_string
extern int_to_hex_string
extern int_to_bin_string
extern slice_buffer
extern print_string_stack_coord_locals

program_proper_calls_main:

	call print_proper

	mov rbx, rsp
	push rbx
	push buffer
	call int_to_string
	add rsp, 8

	push buffer
	call print_old
	add rsp, 4

	mov dx, 0xFF11
	push dx
;	sub rsp, 8

	mov rbx, rsp
	push rbx
	push buffer
	call int_to_string
	add rsp, 8

	push buffer
	call print_old
	add rsp, 4

	pop dx
;	add rsp, 8

	push rdx
	push buffer
	call int_to_hex_string
	add rsp, 8

	push buffer
	call print_old
	add rsp, 4


	ret

print_proper:
	ret

; (+4) pointer to string
print_old:
	push qword [rsp+4]
	call console_writeline
	add rsp, 4

	ret