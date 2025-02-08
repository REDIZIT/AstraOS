	extern _binary_src_font_psf_start
	section .data
	str__Hello_from_Astra_ db 18, " Hello from Astra ", 0
	
	section .text
	global main
	
main:
	call program.main
halt:
	hlt
	jmp halt
	
	
	
Console.SetPixel:
	push rbp
	mov rbp, rsp
	
	sub rsp, 8 ; allocate ptr 'fb' at [rbp-8]
	mov qword [rbp-8], 0xfffffffffc000000
	
; -- Literal = 4
	sub rsp, 8 ; allocate long 'anon_1' at [rbp-16]
	mov qword [rbp-16], 4
	
	sub rsp, 8 ; allocate int 'anon_2' at [rbp-24]
	mov rdi, [rbp+32]
	mov rax, [rbp-16]
	mul rdi
	mov [rbp-24], rax
	
; -- Literal = 3200
	sub rsp, 8 ; allocate long 'anon_3' at [rbp-32]
	mov qword [rbp-32], 3200
	
	sub rsp, 8 ; allocate int 'anon_4' at [rbp-40]
	mov rdi, [rbp+24]
	mov rax, [rbp-32]
	mul rdi
	mov [rbp-40], rax
	
	sub rsp, 8 ; allocate int 'anon_5' at [rbp-48]
	mov rbx, [rbp-24]
	mov rdx, [rbp-40]
	add rbx, rdx
	mov [rbp-48], rbx
	
; -- Shift pointer fb by anon_5
; -- Ptr shift
	mov rbx, [rbp-8]
	mov rdx, [rbp-48]
	add rbx, rdx
	mov [rbp-8], rbx
	
; -- PtrSet c to fb
; -- Ptr set
	mov rbx, [rbp-8]
	mov rdx, [rbp+16]
	mov dword [rbx], edx
	
	
	mov rsp, rbp
	pop rbp
	ret
	
	
	
Console.PutChar:
	push rbp
	mov rbp, rsp
	
	sub rsp, 8 ; allocate ptr '_binary_src_font_psf_start' at [rbp-8]
	mov qword [rbp-8], _binary_src_font_psf_start
	
; -- Literal = 32
	sub rsp, 8 ; allocate long 'anon_1' at [rbp-16]
	mov qword [rbp-16], 32
	
	sub rsp, 8 ; allocate int 'anon_2' at [rbp-24]
	mov rbx, [rbp-8]
	mov rdx, [rbp-16]
	add rbx, rdx
	mov [rbp-24], rbx
	
	sub rsp, 8 ; allocate int 'fontStart' at [rbp-32]
	mov qword rbx, [rbp-24]
	mov qword [rbp-32], rbx
	
; -- Literal = 0xfffffffffc000000
	sub rsp, 8 ; allocate long 'anon_3' at [rbp-40]
	mov qword [rbp-40], 0xfffffffffc000000
	
; -- Literal = 4
	sub rsp, 8 ; allocate long 'anon_4' at [rbp-48]
	mov qword [rbp-48], 4
	
; -- Literal = 8
	sub rsp, 8 ; allocate long 'anon_5' at [rbp-56]
	mov qword [rbp-56], 8
	
	sub rsp, 8 ; allocate int 'anon_6' at [rbp-64]
	mov rdi, [rbp+24]
	mov rax, [rbp-56]
	mul rdi
	mov [rbp-64], rax
	
; -- Literal = 16
	sub rsp, 8 ; allocate long 'anon_7' at [rbp-72]
	mov qword [rbp-72], 16
	
	sub rsp, 8 ; allocate int 'anon_8' at [rbp-80]
	mov rdi, [rbp+16]
	mov rax, [rbp-72]
	mul rdi
	mov [rbp-80], rax
	
; -- Literal = 800
	sub rsp, 8 ; allocate long 'anon_9' at [rbp-88]
	mov qword [rbp-88], 800
	
	sub rsp, 8 ; allocate int 'anon_10' at [rbp-96]
	mov rdi, [rbp-80]
	mov rax, [rbp-88]
	mul rdi
	mov [rbp-96], rax
	
	sub rsp, 8 ; allocate int 'anon_11' at [rbp-104]
	mov rbx, [rbp-64]
	mov rdx, [rbp-96]
	add rbx, rdx
	mov [rbp-104], rbx
	
	sub rsp, 8 ; allocate int 'anon_12' at [rbp-112]
	mov rdi, [rbp-48]
	mov rax, [rbp-104]
	mul rdi
	mov [rbp-112], rax
	
	sub rsp, 8 ; allocate int 'anon_13' at [rbp-120]
	mov rbx, [rbp-40]
	mov rdx, [rbp-112]
	add rbx, rdx
	mov [rbp-120], rbx
	
	sub rsp, 8 ; allocate int 'screenStart' at [rbp-128]
	mov qword rbx, [rbp-120]
	mov qword [rbp-128], rbx
	
; -- Literal = 0x10
	sub rsp, 8 ; allocate long 'anon_14' at [rbp-136]
	mov qword [rbp-136], 0x10
	
	sub rsp, 8 ; allocate int 'anon_15' at [rbp-144]
	mov rdi, [rbp+32]
	mov rax, [rbp-136]
	mul rdi
	mov [rbp-144], rax
	
; -- Shift pointer fontStart by anon_15
; -- Ptr shift
	mov rbx, [rbp-32]
	mov rdx, [rbp-144]
	add rbx, rdx
	mov [rbp-32], rbx
	
; -- ------------------------ Node_For begin
; -- ---------------- for.declaration
	sub rsp, 8 ; allocate int 'y' at [rbp-152]
	mov qword [rbp-152], 0
	
; -- ---------------- for.condition
for_condition:
	push rsp ; allocate saver at [rbp-160]
	
; -- Literal = 16
	sub rsp, 8 ; allocate long 'anon_16' at [rbp-168]
	mov qword [rbp-168], 16
	
	sub rsp, 8 ; allocate bool 'anon_17' at [rbp-176]
	mov rbx, [rbp-152]
	mov rdx, [rbp-168]
	cmp rbx, rdx
	mov rbx, 0
	setl bl
	mov [rbp-176], rbx
	
; -- ---------------- for.condition_check
	mov rbx, [rbp-176]
	cmp rbx, 0
	jle for_end
	
; -- ---------------- for.body
; -- Get value from fontStart
	sub rsp, 8 ; allocate long 'anon_18' at [rbp-184]
; -- Ptr get
	mov rbx, [rbp-32]
	mov rdx, [rbx]
	mov qword [rbp-184], rdx
	
	sub rsp, 8 ; allocate long 'row' at [rbp-192]
	mov qword rbx, [rbp-184]
	mov qword [rbp-192], rbx
	
; -- Literal = 1
	sub rsp, 8 ; allocate long 'anon_19' at [rbp-200]
	mov qword [rbp-200], 1
	
; -- Shift pointer fontStart by anon_19
; -- Ptr shift
	mov rbx, [rbp-32]
	mov rdx, [rbp-200]
	add rbx, rdx
	mov [rbp-32], rbx
	
; -- Literal = 1
	sub rsp, 8 ; allocate long 'anon_20' at [rbp-208]
	mov qword [rbp-208], 1
	
; -- Literal = 7
	sub rsp, 8 ; allocate long 'anon_21' at [rbp-216]
	mov qword [rbp-216], 7
	
	sub rsp, 8 ; allocate int 'anon_22' at [rbp-224]
	mov rdx, [rbp-208]
	mov rcx, [rbp-216]
	shl rdx, cl
	mov [rbp-224], rdx
	
	sub rsp, 8 ; allocate int 'mask' at [rbp-232]
	mov qword rbx, [rbp-224]
	mov qword [rbp-232], rbx
	
; -- ------------------------ Node_For begin
; -- ---------------- for.declaration
	sub rsp, 8 ; allocate int 'x' at [rbp-240]
	mov qword [rbp-240], 0
	
; -- ---------------- for.condition
for_condition_2:
	push rsp ; allocate saver at [rbp-248]
	
; -- Literal = 8
	sub rsp, 8 ; allocate long 'anon_23' at [rbp-256]
	mov qword [rbp-256], 8
	
	sub rsp, 8 ; allocate bool 'anon_24' at [rbp-264]
	mov rbx, [rbp-240]
	mov rdx, [rbp-256]
	cmp rbx, rdx
	mov rbx, 0
	setl bl
	mov [rbp-264], rbx
	
; -- ---------------- for.condition_check
	mov rbx, [rbp-264]
	cmp rbx, 0
	jle for_end_2
	
; -- ---------------- for.body
	sub rsp, 8 ; allocate int 'c' at [rbp-272]
	mov qword [rbp-272], 0x000dbc79
	
	sub rsp, 8 ; allocate int 'anon_25' at [rbp-280]
	mov rdx, [rbp-192]
	mov rbx, [rbp-232]
	and rdx, rbx
	mov [rbp-280], rdx
	
; -- if anon_25
	mov rbx, [rbp-280]
	cmp rbx, 0
	jle if_false
; -- generating target for assign
; -- generating value for assign
; -- Assign c = 0x00ffffff
	mov qword [rbp-272], 0x00ffffff
if_false:
	
; -- PtrSet c to screenStart
; -- Ptr set
	mov rbx, [rbp-128]
	mov rdx, [rbp-272]
	mov dword [rbx], edx
	
; -- Literal = 4
	sub rsp, 8 ; allocate long 'anon_26' at [rbp-288]
	mov qword [rbp-288], 4
	
; -- Shift pointer screenStart by anon_26
; -- Ptr shift
	mov rbx, [rbp-128]
	mov rdx, [rbp-288]
	add rbx, rdx
	mov [rbp-128], rbx
	
; -- generating target for assign
; -- generating value for assign
; -- Literal = 1
	sub rsp, 8 ; allocate long 'anon_27' at [rbp-296]
	mov qword [rbp-296], 1
	
	sub rsp, 8 ; allocate int 'anon_28' at [rbp-304]
	mov rdx, [rbp-232]
	mov rcx, [rbp-296]
	shr rdx, cl
	mov [rbp-304], rdx
	
; -- Assign mask = anon_28
	mov qword rbx, [rbp-304]
	mov qword [rbp-232], rbx
	
; -- ---------------- for.advance
; -- generating target for assign
; -- generating value for assign
; -- Literal = 1
	sub rsp, 8 ; allocate long 'anon_29' at [rbp-312]
	mov qword [rbp-312], 1
	
	sub rsp, 8 ; allocate int 'anon_30' at [rbp-320]
	mov rbx, [rbp-240]
	mov rdx, [rbp-312]
	add rbx, rdx
	mov [rbp-320], rbx
	
; -- Assign x = anon_30
	mov qword rbx, [rbp-320]
	mov qword [rbp-240], rbx
	
	mov rsp, [rbp-248] ; restore saver
	jmp for_condition_2
	
for_end_2:
	pop rsp ; deallocate saver
; -- ------------------------ Node_For end
; -- Literal = 3168
	sub rsp, 8 ; allocate long 'anon_31' at [rbp-336]
	mov qword [rbp-336], 3168
	
; -- Shift pointer screenStart by anon_31
; -- Ptr shift
	mov rbx, [rbp-128]
	mov rdx, [rbp-336]
	add rbx, rdx
	mov [rbp-128], rbx
	
; -- ---------------- for.advance
; -- generating target for assign
; -- generating value for assign
; -- Literal = 1
	sub rsp, 8 ; allocate long 'anon_32' at [rbp-344]
	mov qword [rbp-344], 1
	
	sub rsp, 8 ; allocate int 'anon_33' at [rbp-352]
	mov rbx, [rbp-152]
	mov rdx, [rbp-344]
	add rbx, rdx
	mov [rbp-352], rbx
	
; -- Assign y = anon_33
	mov qword rbx, [rbp-352]
	mov qword [rbp-152], rbx
	
	mov rsp, [rbp-160] ; restore saver
	jmp for_condition
	
for_end:
	pop rsp ; deallocate saver
; -- ------------------------ Node_For end
	mov rsp, rbp
	pop rbp
	ret
	
	
	
program.main:
	push rbp
	mov rbp, rsp
	
	sub rsp, 8 ; allocate long 'anon_1' at [rbp-8]
	
; -- new Console
	sub rsp, 8 ; allocate Console 'anon_2' at [rbp-16]
; -- heap alloc
	mov qword [rbp-8], 0x110
	mov rbx, [0x100]
	add rbx, 1
	mov [0x100], rbx
	sub rsp, 8 ; allocate long 'console' at [rbp-24]
	mov qword rbx, [rbp-8]
	mov qword [rbp-24], rbx
	
	sub rsp, 8 ; allocate string 'str' at [rbp-32]
	mov qword [rbp-32], str__Hello_from_Astra_
	
; -- ------------------------ Node_For begin
; -- ---------------- for.declaration
	sub rsp, 8 ; allocate int 'i' at [rbp-40]
	mov qword [rbp-40], 0
	
; -- ---------------- for.condition
for_condition_3:
	push rsp ; allocate saver at [rbp-48]
	
; -- String get length str
	sub rsp, 8 ; allocate ptr 'anon_3' at [rbp-56]
; -- ToPtr str (heap data)
	mov rbx, rbp
	add rbx, -32
	mov rbx, [rbx]
	mov [rbp-56], rbx
	sub rsp, 8 ; allocate byte 'anon_4' at [rbp-64]
; -- Ptr get
	mov rbx, [rbp-56]
	mov rdx, [rbx]
	mov byte [rbp-64], dl
	sub rsp, 8 ; allocate bool 'anon_5' at [rbp-72]
	mov rbx, [rbp-40]
	mov rdx, [rbp-64]
	cmp rbx, rdx
	mov rbx, 0
	setl bl
	mov [rbp-72], rbx
	
; -- ---------------- for.condition_check
	mov rbx, [rbp-72]
	cmp rbx, 0
	jle for_end_3
	
; -- ---------------- for.body
; -- String get char str[i]
	sub rsp, 8 ; allocate ptr 'anon_6' at [rbp-80]
; -- ToPtr str (heap data)
	mov rbx, rbp
	add rbx, -32
	mov rbx, [rbx]
	mov [rbp-80], rbx
; -- Ptr shift
	mov rbx, [rbp-80]
	mov rdx, [rbp-40]
	add rbx, rdx
	add rbx, 1 ; additionalShift
	mov [rbp-80], rbx
	sub rsp, 8 ; allocate byte 'anon_7' at [rbp-88]
; -- Ptr get
	mov rbx, [rbp-80]
	mov rdx, [rbx]
	mov byte [rbp-88], dl
	sub rsp, 8 ; allocate byte 'c' at [rbp-96]
	mov qword rbx, [rbp-88]
	mov qword [rbp-96], rbx
	
; -- Astra.Compilation.Node_FieldAccess.PutChar()
; -- arguments generation
; -- Literal = 2
	sub rsp, 8 ; allocate long 'anon_8' at [rbp-104]
	mov qword [rbp-104], 2
	
	sub rsp, 8 ; allocate int 'anon_9' at [rbp-112]
	mov rbx, [rbp-40]
	mov rdx, [rbp-104]
	add rbx, rdx
	mov [rbp-112], rbx
	
; -- skip literal = 1
; -- arguments pushing
	mov rbx, [rbp-24] ; self
	push rbx
	mov rbx, [rbp-96] ; arg[0] = char
	push rbx
	mov rbx, [rbp-112] ; arg[1] = cx
	push rbx
	mov rbx, 1 ; arg[2] = cy
	push rbx
	call Console.PutChar
	add rsp, 32
	
; -- ---------------- for.advance
; -- generating target for assign
; -- generating value for assign
; -- Literal = 1
	sub rsp, 8 ; allocate long 'anon_10' at [rbp-120]
	mov qword [rbp-120], 1
	
	sub rsp, 8 ; allocate int 'anon_11' at [rbp-128]
	mov rbx, [rbp-40]
	mov rdx, [rbp-120]
	add rbx, rdx
	mov [rbp-128], rbx
	
; -- Assign i = anon_11
	mov qword rbx, [rbp-128]
	mov qword [rbp-40], rbx
	
	mov rsp, [rbp-48] ; restore saver
	jmp for_condition_3
	
for_end_3:
	pop rsp ; deallocate saver
; -- ------------------------ Node_For end
	mov rsp, rbp
	pop rbp
	ret