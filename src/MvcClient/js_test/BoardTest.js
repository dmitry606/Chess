describe('Board', function () {
	var _initialBoardString = '{"History":[],"White":{"PieceStrings":["pa2","pb2","pc2","pd2","pe2","pf2","pg2","ph2","Ra1","Nb1","Bc1","Qd1","Ke1","Bf1","Ng1","Rh1"]},"Black":{"PieceStrings":["pa7","pb7","pc7","pd7","pe7","pf7","pg7","ph7","Ra8","Nb8","Bc8","Qd8","Ke8","Bf8","Ng8","Rh8"]}}';
	var _initialBoard = JSON.parse(_initialBoardString);

	//white deleted: pa2 pe2 Nb1 Qd1
	//white added: Nc4 pg3
	//black deleted: pg7 Rh8
	//black added: pg9
	var _changedBoardString = '{"History":[],"White":{"PieceStrings":["Nc4", "pg3", "pb2","pc2","pd2","pf2","pg2","ph2","Ra1","Bc1","Ke1","Bf1","Ng1","Rh1"]},"Black":{"PieceStrings":["pa7","pb7","pc7","pd7","pe7","pf7","ph7","Ra8","Nb8","Bc8","Qd8","Ke8","Bf8","Ng8", "pg9"]}}';
	var _changedBoard = JSON.parse(_changedBoardString);

	var gameIdExp = 'unit_test_id_11';
	_initialBoard.Id = gameIdExp;

	beforeEach(module('app')); 
	var board, gamesServiceStub;

	beforeEach(inject(function ($rootScope) {
		gamesServiceStub = {};
		gamesServiceStub.getGame = function (id, onSuccess) {
			onSuccess(_initialBoard);
		};
		spyOn(gamesServiceStub, 'getGame').and.callThrough();
		board = new Board(gameIdExp, gamesServiceStub);
	}));

	it('correctly sets initial board', function () {
		board.update();

		expect(gamesServiceStub.getGame).toHaveBeenCalled();
		expect(board.currentGame).not.toBeNull();
		expect(board.d1).toEqual({ color: 'White', pieceChar: 'Q' });
		expect(board.h8).toEqual({ color: 'Black', pieceChar: 'R' });
		expect(board.c4).toBeUndefined();
		expect(board.g3).toBeUndefined();
		expect(board.g9).toBeUndefined();
	});

	it('updates with new pieces and delete old ones', function () {
		board.update();
		board.setGame(_changedBoard);

		expect(board.d1).toBeUndefined();
		expect(board.h8).toBeUndefined();
		expect(board.e2).toBeUndefined();
		expect(board.g7).toBeUndefined();
		expect(board.c4).toEqual({ color: 'White', pieceChar: 'N' });
		expect(board.g3).toEqual({ color: 'White', pieceChar: 'p' });
		expect(board.g9).toEqual({ color: 'Black', pieceChar: 'p' });
	});
});