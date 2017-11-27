Feature: Company
	Eu como usuario
	Gostaria de visualizar as informações dos funcionarios de uma empresa
	Para que eu consiga gerenciar os funcionarios

@CompanyController
Scenario: Listar todos os funcionarios de uma empresa
	When Chamo o controller api/company/ [GET] method
	Then Devo vizualizar as informações de um ou mais funcionarios.
	| Name  | LastName | Genre     | Birthday   | CompanyId                            | EmployerId                           |
	| Bruce | Wayne    | Masculino | 25/05/1976 | 80f3a63f-3b8d-4b91-a08f-efd19ef379de | 80f3a63f-3b8d-4b91-a08f-efd19ef379de |
