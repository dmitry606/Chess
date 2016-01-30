describe('GamesService', function () {
	var apiBase = '/api/games/';
	var gameIdExp = 'unit_test_id_11';

	beforeEach(module('app'));
	var $httpBackend, GamesService;
	

	beforeEach(inject(function (_$httpBackend_, _GamesService_) {
		$httpBackend = _$httpBackend_;
		$httpBackend.expectGET(apiBase + gameIdExp).respond({});

		GamesService = _GamesService_;
	}));

	it('gets board from the server', function () {
		var received;
		GamesService.getGame(gameIdExp, function (game) {
			received = game;
		});
		$httpBackend.flush();

		expect(received).toEqual({ Id: gameIdExp });
	});

	it('caches game on get', function () {
		var first;
		var second;

		GamesService.getGame(gameIdExp, function (g) { first = g; });
		$httpBackend.flush();
		GamesService.getGame(gameIdExp, function (g) { second = g });

		expect(first).toEqual(second);
		expect(first).not.toBe(second); //= returns a copy
	});

});