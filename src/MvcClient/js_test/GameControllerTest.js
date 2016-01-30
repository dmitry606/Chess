describe('GameController test', function () {
	var _initialBoardString = '{"History":[],"White":{"PieceStrings":["pa2","pb2","pc2","pd2","pe2","pf2","pg2","ph2","Ra1","Nb1","Bc1","Qd1","Ke1","Bf1","Ng1","Rh1"]},"Black":{"PieceStrings":["pa7","pb7","pc7","pd7","pe7","pf7","pg7","ph7","Ra8","Nb8","Bc8","Qd8","Ke8","Bf8","Ng8","Rh8"]}}';
	var _initialBoard = JSON.parse(_initialBoardString);
	var gameIdExp = 'unit_test_id_11';
	_initialBoard.Id = gameIdExp;

	beforeEach(module('app')); 
	var ctrl, scope, gamesServiceStub;

	beforeEach(inject(function ($rootScope, $routeParams, $controller) {
		$routeParams.id = gameIdExp;
		scope = $rootScope.$new();
		gamesServiceStub = {};
		gamesServiceStub.getGame = function (id, onSuccess) {
			onSuccess(_initialBoard);
		};
		spyOn(gamesServiceStub, 'getGame').and.callThrough();
		ctrl = $controller('GameController', { $scope: scope, GamesService: gamesServiceStub });
	}));

	it('should fetch the game on start', function () {
		expect(gamesServiceStub.getGame).toHaveBeenCalled();
		expect(ctrl.currentGame).not.toBeNull();
	});

	it('sets board into correct state', function () {
		expect(scope.board).not.toBeNull();
		expect(scope.board.h8).toEqual({ color: 'Black', pieceChar: 'R' });
		expect(scope.board.d1).toEqual({ color: 'White', pieceChar: 'Q' });
	});
});